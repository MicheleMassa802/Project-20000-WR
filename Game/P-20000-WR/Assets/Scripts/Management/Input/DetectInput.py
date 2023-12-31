import cv2
import numpy as np
from Utils import GetLimits, DrawStrikeZone, DrawBatLine


# CONSTANTS

# in cm
IRL_DIM = (206, 154)  # real life WxH dimensions of "batting cage"
SZ_W = 43
SZ_H = 65
KNEE_HEIGHT = 25  # cm from cam bottom to knees

# in pixels
IMAGE_DIM = (640, 480)  # WxH of captured image of "batting cage"
SZ_W_PX = IMAGE_DIM[0] * SZ_W // IRL_DIM[0]
SZ_H_PX = IMAGE_DIM[1] * SZ_H // IRL_DIM[1]
KNEE_HEIGHT_PX = IMAGE_DIM[1] * KNEE_HEIGHT // IRL_DIM[1]

# colors in BGR
BLUE = (255, 0, 0)
GREEN = (0, 255, 0)
RED = (50, 40, 180)
YELLOW = (0, 255, 255)


###############################################################################
# File for determining the position and movement of the "bat" object
###############################################################################


"""
Filter video input for the color RED to get a mask of the bat object using cv2 contours.
This uses a bat made up of a stick with a single large red zone in its hitting section.

Args:
- videoSourceNum: index of the camera to use for video input
- righty: boolean keeping track of whether the batter is a righty or a lefty
"""


def WebCamColorFilteringV1(videoSourceNum: int, righty: bool):

    bat_pos = None
    bat_speed = None
    bat_dir = None

    capture = cv2.VideoCapture(videoSourceNum)

    while True:
        _, frame = capture.read()
        frame = cv2.flip(frame, 1)
        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

        # manual thresholds based on testing
        lower_red, upper_red = GetLimits(RED)

        # everything within these ranges
        mask = cv2.inRange(hsv, lower_red, upper_red)

        # calculate the contours and their centers such that we can get the position of the bat
        contours, _ = cv2.findContours(
            mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        bat_pos, bat_speed, bat_dir = BatDataV1(
            frame, contours, bat_pos, bat_speed, bat_dir, righty)
        cv2.circle(frame, bat_pos, 8, BLUE, -1)
        DrawStrikeZone(frame)
        DrawBatLine(frame, bat_pos, righty)

        cv2.imshow("OG", frame)
        cv2.imshow("Mask", mask)

        # exit loop
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()  # stop using camera
    cv2.destroyAllWindows()


"""
Filter video input for the color RED to get a mask of the bat using cv2 contours.
This uses a bat made up of a stick with two small red zones, one at the base
and one at the top of the bat's hitting section.

Args:
- videoSourceNum: index of the camera to use for video input
- righty: boolean keeping track of whether the batter is a righty or a lefty
"""


def WebCamColorFilteringV2(videoSourceNum: int, righty: bool):

    bat_pos = None
    bat_speed = None
    bat_dir = None

    capture = cv2.VideoCapture(videoSourceNum)

    while True:
        _, frame = capture.read()
        frame = cv2.flip(frame, 1)
        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

        # manual thresholds based on testing
        lower_red, upper_red = GetLimits(RED, 30)

        # everything within these ranges
        mask = cv2.inRange(hsv, lower_red, upper_red)

        # calculate the contours and their centers such that we can get the position of the bat
        contours, _ = cv2.findContours(
            mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        bat_pos, bat_speed, bat_dir = BatDataV2(
            frame, contours, bat_pos, bat_speed, bat_dir, righty)

        cv2.circle(frame, bat_pos, 8, BLUE, -1)
        DrawStrikeZone(frame)
        # DrawBatLine(frame, bat_pos, righty)

        cv2.imshow("OG", frame)
        cv2.imshow("Mask", mask)

        # exit loop
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()  # stop using camera
    cv2.destroyAllWindows()


"""
Helper to draw polygon (for V1 iteration) that makes up the bat,
find its speed, direction and position.

Args:
- frame: video frame to analyze
- contours: nparray of the detected red contours by cv2
- prev pos/speed/dir: position, speed, and direction values of the last frame of the video
- righty: boolean keeping track of whether the batter is a righty or a lefty

Return:
- this frame's position, speed, and direction
"""


def BatDataV1(frame, contours, prev_pos, prev_speed, prev_dir, righty):

    curr_pos = prev_pos  # default
    curr_speed = prev_speed
    curr_direction = prev_dir

    # draw a rectangle for the largest contour detected (bat)
    if (contours):
        largest_contour = max(contours, key=cv2.contourArea)
        x, y, w, h = cv2.boundingRect(largest_contour)
        cv2.rectangle(frame, (x, y), (x+w, y+h), GREEN, 2)

        M = cv2.moments(largest_contour)
        if M['m00'] != 0:
            cx = int(M['m10'] / M['m00'])
            cy = int(M['m01'] / M['m00'])
            curr_pos = np.array([cx, cy])
        else:
            cx, cy = 0, 0
            curr_pos = prev_pos

        # calculate speed and direction
        if prev_pos is not None:
            diff = curr_pos - prev_pos
            curr_speed = np.linalg.norm(diff)
            curr_direction = np.arctan2(diff[1], diff[0])

            font = cv2.FONT_HERSHEY_SIMPLEX
            cv2.putText(frame, str(curr_pos), (10, 30),
                        font, 1, BLUE, 1, cv2.LINE_AA)
            cv2.putText(frame, str(curr_speed), (10, 70),
                        font, 1, BLUE, 1, cv2.LINE_AA)
            cv2.putText(frame, str(curr_direction), (10, 100),
                        font, 1, BLUE, 1, cv2.LINE_AA)

    # Update the position of the bat
    return curr_pos, curr_speed, curr_direction


"""
Helper to draw the 2 polygons (for V2 iteration) that make up the bat,
find its speed, direction and position.

Args:
- frame: video frame to analyze
- contours: nparray of the detected red contours by cv2
- prev pos/speed/dir: position, speed, and direction values of the last frame of the video
- righty: boolean keeping track of whether the batter is a righty or a lefty

Return:
- this frame's position, speed, and direction
"""


def BatDataV2(frame, contours, prev_pos, prev_speed, prev_dir, righty):

    curr_pos = prev_pos  # default
    curr_speed = prev_speed
    curr_direction = prev_dir

    if len(contours) > 1:

        # draw circles over the two largest contours
        contour1 = max(contours, key=cv2.contourArea)
        contours = [c for c in contours if not np.array_equal(c, contour1)]
        contour2 = max(contours, key=cv2.contourArea)

        (x1, y1), rad1 = cv2.minEnclosingCircle(contour1)
        (x2, y2), rad2 = cv2.minEnclosingCircle(contour2)

        # determine the top blob
        (top, bottom) = AssignTopBottom(
            (int(x1), int(y1)), (int(x2), int(y2)), righty)
        cv2.circle(frame, top, int(rad1), GREEN, 2)
        cv2.circle(frame, bottom, int(rad2), YELLOW, 2)

        # calculate the mid-point of the bat
        curr_pos_x = int((top[0] + bottom[0]) // 2)
        curr_pos_y = int((top[1] + bottom[1]) // 2)
        curr_pos = np.array([curr_pos_x, curr_pos_y])

        # calculate speed and direction
        if prev_pos is not None:
            diff = curr_pos - prev_pos
            curr_speed = np.linalg.norm(diff)
            curr_direction = np.arctan2(diff[1], diff[0])

            font = cv2.FONT_HERSHEY_SIMPLEX
            cv2.putText(frame, str(curr_pos), (10, 30),
                        font, 1, BLUE, 1, cv2.LINE_AA)
            cv2.putText(frame, str(curr_speed), (10, 70),
                        font, 1, BLUE, 1, cv2.LINE_AA)
            cv2.putText(frame, str(curr_direction), (10, 100),
                        font, 1, BLUE, 1, cv2.LINE_AA)

    # Update the position of the bat
    return curr_pos, curr_speed, curr_direction


"""
Helper for the V2 implementation that helps identify which 'blob' is the 
top one and which is the bottom (base) one of the batting object in an image.

Args:
- coords1, coords2: sets of (x,y) coordinates of each blob
- righty: boolean keeping track of whether the batter is a righty or a lefty

Return:
- tuple of 2 (coord1,coord2) coordinate sets, where the first element represents the
  top coordinate and the second the bottom one
"""


def AssignTopBottom(coords1, coords2, righty):

    if righty:

        # higher y and higher x >> higher x >> higher y
        if coords1[1] >= coords2[1] and coords1[0] >= coords2[0]:
            return (coords1, coords2)
        else:
            return (coords1, coords2) if coords1[0] >= coords2[0] else (coords2, coords1)

    else:

        # higher y and lower x >> lower x >> higher y
        if coords1[1] >= coords2[1] and coords1[0] <= coords2[0]:
            return (coords1, coords2)
        else:
            return (coords1, coords2) if coords1[0] <= coords2[0] else (coords2, coords1)


"""
Performs and iteration of the WHILE TRUE CV2 loop when filtering for a color
between lower_red and upper_red while calculating some extra data.

Args:
- capture: cv2 video capture
- bat_data: a dictionary of form {'pos': (x,y), 'spd': s, 'dir': d}
- lower/upper red: HSV formatted tupples detailing the ranges to look for
- righty: boolean keeping track of whether the batter is a righty or a lefty

Return:
- the current frame, its mask, and the dictionary of bat data.
"""


def WebCamColorFilteringIterationV1(capture, bat_data, lower_red, upper_red, righty):

    # the bat data from the previous frame
    bat_pos = bat_data['pos']
    bat_speed = bat_data['spd']
    bat_dir = bat_data['dir']

    # while True -> here goes what would go inside the usual loop
    _, frame = capture.read()
    frame = cv2.flip(frame, 1)
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    # everything within these ranges
    mask = cv2.inRange(hsv, lower_red, upper_red)

    # calculate the contours and their centers such that we can get the position of the bat
    contours, _ = cv2.findContours(
        mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    bat_pos, bat_speed, bat_dir = BatDataV1(
        frame, contours, bat_pos, bat_speed, bat_dir, righty)
    DrawStrikeZone(frame)
    DrawBatLine(frame, bat_pos, righty)

    return frame, mask, {'pos': bat_pos, 'spd': bat_speed, 'dir': bat_dir}


###############################################################################
# Main Function
###############################################################################

"""
Function to be called when running this script.

Mostly just for testing stuff within this script, the real finalized implementation
is whatever is called within the SendInput.py script.
"""


def Main():

    # live video
    print(f"\nShowing live video feed.\n\nPress Q to exit.")
    WebCamColorFilteringV2(0, True)


if __name__ == "__main__":
    Main()

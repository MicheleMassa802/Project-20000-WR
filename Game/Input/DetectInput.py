import cv2
import numpy as np
from Utils import GetLimits


# CONSTANTS

# in cm
IRL_DIM = (206, 154)  # w by h
SZ_W = 43
SZ_H = 65
KNEE_HEIGHT = 25  # cm from cam bottom to knees

# in pixels
IMAGE_DIM = (640, 480)  # w by h
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
Filter the color RED to get a mask of the bat using cv2 contours.
"""


def WebCamColorFilteringV1(videoSourceNum: int):

    bat_pos = None
    bat_speed = None
    bat_dir = None

    capture = cv2.VideoCapture(videoSourceNum)

    while True:
        _, frame = capture.read()
        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

        # manual thresholds based on testing
        lower_red, upper_red = GetLimits(RED)

        # everything within these ranges
        mask = cv2.inRange(hsv, lower_red, upper_red)

        # calculate the contours and their centers such that we can get the position of the bat
        contours, _ = cv2.findContours(
            mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        bat_pos, bat_speed, bat_dir = BatData(
            frame, contours, bat_pos, bat_speed, bat_dir)
        cv2.circle(frame, bat_pos, 1, BLUE, -1)
        DrawStrikeZone(frame)

        cv2.imshow("OG", frame)
        cv2.imshow("Mask", mask)

        # exit loop
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()  # stop using camera
    cv2.destroyAllWindows()


"""
Filter the color YELLOW to get a mask of the bat using cv2 contours.
"""


def WebCamColorFilteringV2(videoSourceNum: int):

    bat_pos = None
    bat_speed = None
    bat_dir = None

    capture = cv2.VideoCapture(videoSourceNum)

    # manual thresholds based on testing
    lower_yellow, upper_yellow = GetLimits(YELLOW)

    while True:
        _, frame = capture.read()
        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

        # everything within these ranges
        mask = cv2.inRange(hsv, lower_yellow, upper_yellow)

        # calculate the contours and their centers such that we can get the position of the bat
        contours, _ = cv2.findContours(
            mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        bat_pos, bat_speed, bat_dir = BatData(
            frame, contours, bat_pos, bat_speed, bat_dir)
        DrawStrikeZone(frame)

        cv2.imshow("OG", frame)
        cv2.imshow("Mask", mask)

        # exit loop
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()  # stop using camera
    cv2.destroyAllWindows()


"""
Performs and iteration of the WHILE TRUE CV2 loop when filtering for a color
between lower_red and upper_red while calculating some extra data.

Return the current frame, its mask, and the tuple of bat data.
"""


def WebCamColorFilteringIteration(capture, bat_data, lower_red, upper_red):

    # the bat data from the previous frame
    bat_pos = bat_data['pos']
    bat_speed = bat_data['spd']
    bat_dir = bat_data['dir']

    # while True -> here goes what would go inside the usual loop
    _, frame = capture.read()
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    # everything within these ranges
    mask = cv2.inRange(hsv, lower_red, upper_red)

    # calculate the contours and their centers such that we can get the position of the bat
    contours, _ = cv2.findContours(
        mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    bat_pos, bat_speed, bat_dir = BatData(
        frame, contours, bat_pos, bat_speed, bat_dir)
    DrawStrikeZone(frame)

    return frame, mask, {'pos': bat_pos, 'spd': bat_speed, 'dir': bat_dir}


"""
Helper to draw polygon that makes up the bat, find its speed,
direction and position.

Returns the new bat position
"""


def BatData(frame, contours, prev_pos, prev_speed, prev_dir):

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
Helper to draw Strike Zone
"""


def DrawStrikeZone(frame):

    centerX, centerY = IMAGE_DIM[0] // 2, IMAGE_DIM[1] - \
        (KNEE_HEIGHT_PX + (SZ_H_PX//2))
    top_left = (centerX - (SZ_W_PX//2), centerY - (SZ_H_PX//2))
    bottom_right = (centerX + (SZ_W_PX//2), centerY + (SZ_H_PX//2))

    cv2.rectangle(frame, top_left, bottom_right, RED, 2)


###############################################################################
# Main Function
###############################################################################


def Main():

    # live video
    print(f"\nShowing live video feed.\n\nPress Q to exit.")
    WebCamColorFilteringV1(0)


if __name__ == "__main__":
    Main()

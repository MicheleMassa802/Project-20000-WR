import cv2
import numpy as np

###############################################################################
# File for determining the position and movement of the "bat" object
###############################################################################


"""
Filter colors to get a mask of the bat using cv2's opening method.
"""


def WebCamColorFiltering(videoSourceNum: int):

    capture = cv2.VideoCapture(videoSourceNum)

    while True:
        _, frame = capture.read()

        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

        # manual thresholds based on testing
        lower_red = np.array([150, 150, 50])
        upper_red = np.array([180, 255, 150])

        # everything within these ranges
        mask = cv2.inRange(hsv, lower_red, upper_red)
        result = cv2.bitwise_and(frame, frame, mask=mask)

        kernel = np.ones((5, 5), np.uint8)  # apply on the mask
        opening = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel)  # worked well

        cv2.imshow("Result", result)
        cv2.imshow("Opening", opening)

        # exit loop
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()  # stop using camera
    cv2.destroyAllWindows()


"""
Filter colors to get a mask of the bat using cv2 contours.
"""


def WebCamColorFilteringV2(videoSourceNum: int):

    marker_positions = None

    capture = cv2.VideoCapture(videoSourceNum)

    while True:
        _, frame = capture.read()

        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

        # manual thresholds based on testing
        lower_red = np.array([0, 50, 50])
        upper_red = np.array([10, 255, 255])

        # everything within these ranges
        mask = cv2.inRange(hsv, lower_red, upper_red)

        # calculate the contours and their centers such that we can get the position of the bat
        contours, _ = cv2.findContours(
            mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        result = cv2.bitwise_and(frame, frame, mask=mask)

        cv2.imshow("OG", frame)
        cv2.imshow("Mask", mask)
        cv2.imshow("Result", result)

        # exit loop
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()  # stop using camera
    cv2.destroyAllWindows()


"""
Helper to draw polygons that make up the bat.
"""


def DrawPolygons(frame, pointsDetected):
    return

###############################################################################
# Main Function
###############################################################################


def Main():

    # live video
    print(f"\nShowing live video feed.\n\nPress Q to exit.")
    # WebCamColorFiltering(0)
    WebCamColorFilteringV2(0)


if __name__ == "__main__":
    Main()

import numpy as np
import cv2


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


"""
Function determining the color of the center pixel of an input image.
Used to provide the BGR values for the RED color to detect.

Args:
- img_path: path to the image to analyze from the location of this script

Return: 
- the BGR value
"""


def DetectColor(img_path):

    # read in image
    img = cv2.imread(img_path)

    h, w = img.shape[:2]

    # get coords of middle pixel
    c = w // 2
    r = h // 2

    bgr = img[r, c]
    print(bgr)


"""
Function determining the thresholds for the input color.

Args:
- color: the BGR color to get the limits for
- tolerance: "hue"-based range between lower and upper

Return: 
- the HSV formatted lower and upper limits for the input color.
"""


def GetLimits(color, tolerance=10):

    color = np.uint8([[color]])
    hsv_color = cv2.cvtColor(color, cv2.COLOR_BGR2HSV)

    lower_limit = hsv_color[0][0][0] - tolerance, 100, 100
    upper_limit = hsv_color[0][0][0] + tolerance, 255, 255

    lower_limit = np.array(lower_limit, dtype=np.uint8)
    upper_limit = np.array(upper_limit, dtype=np.uint8)

    return lower_limit, upper_limit


"""
Draws the strike zone in a given frame. No return.

Args:
- frame: video frame to analyze
"""


def DrawStrikeZone(frame):

    centerX, centerY = IMAGE_DIM[0] // 2, IMAGE_DIM[1] - \
        (KNEE_HEIGHT_PX + (SZ_H_PX//2))
    top_left = (centerX - (SZ_W_PX//2), centerY - (SZ_H_PX//2))
    bottom_right = (centerX + (SZ_W_PX//2), centerY + (SZ_H_PX//2))

    cv2.rectangle(frame, top_left, bottom_right, RED, 2)


"""
Helper to draw lines on the screen displaying key info such as bat
position and angle. No return.

Args:
- frame: video frame to analyze
- bat_center: (x,y) coordinates of the detected bat's center
- righty: boolean keeping track of whether the batter is a righty or a lefty
"""


def DrawBatLine(frame, bat_center, righty):

    # "bottom & top" coords
    base_x = IMAGE_DIM[1] // 4 if righty else (IMAGE_DIM[1] * 3) // 4
    base_y = IMAGE_DIM[1] // 2

    # bat end coords

    if bat_center is not None:
        cv2.line(frame, (base_x, base_y),
                 (bat_center[0], bat_center[1]), GREEN, 5)

import cv2
import numpy as np
import matplotlib.pyplot as plt


"""
Read the image at path in grayscale, discard the alpha channel,
and plot the image.
"""


def ReadImageGray(path: str):

    img = cv2.imread(path, cv2.IMREAD_GRAYSCALE)
    cv2.imshow("Image Plotted", img)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

    # plt.imshow(img, cmap='gray')
    # plt.show()

    # cv2.imwrite("NewImage.png", img)


"""
Tinker with image operations given an image to read.
"""


def ImageThresholding(path: str):

    img = cv2.imread(path)
    retval, threshold = cv2.threshold(img, 12, 255, cv2.THRESH_BINARY)

    grayscaled = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    retval2, threshold2 = cv2.threshold(img, 12, 255, cv2.THRESH_BINARY)

    gaus = cv2.adaptiveThreshold(
        grayscaled, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 115, 1)

    cv2.imshow("img", img)
    cv2.imshow("gaus", gaus)
    cv2.waitKey(0)
    cv2.destroyAllWindows()


"""
Read and display live webcam feed.
"""


def ReadWebCamFeed(videoSourceNum: int, saveVideo: bool):

    capture = cv2.VideoCapture(videoSourceNum)

    # for saving video feed
    if saveVideo:
        fourcc = cv2.VideoWriter_fourcc(*'XVID')
        out = cv2.VideoWriter('outputVid.avi', fourcc, 20.0, (640, 480))

    while True:
        ret, frame = capture.read()
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

        if saveVideo:
            out.write(frame)

        cv2.imshow("Live Video Feed", frame)
        cv2.imshow("Live Grayscale Video Feed", gray)

        # exit loop
        if cv2.waitKey(1) & 0xFF == ord('q'):
            # waitKey() returns a 32 bit int corresponding to the key pressed
            # so you bitwise and it to get the last 8 bits and compare that to
            # the ascii value of q
            break

    capture.release()  # stop using camera
    cv2.destroyAllWindows()


"""
Color filtering on video capture.
"""


def WebCamColorFiltering(videoSourceNum: int, saveVideo: bool):

    capture = cv2.VideoCapture(videoSourceNum)

    # for saving video feed
    if saveVideo:
        fourcc = cv2.VideoWriter_fourcc(*'XVID')
        out = cv2.VideoWriter('outputVid.avi', fourcc, 20.0, (640, 480))

    while True:
        ret, frame = capture.read()

        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

        # manual thresholds based on testing
        lower_red = np.array([150, 150, 50])
        upper_red = np.array([180, 255, 150])

        # everything within these ranges
        mask = cv2.inRange(hsv, lower_red, upper_red)
        result = cv2.bitwise_and(frame, frame, mask=mask)

        kernel = np.ones((5, 5), np.uint8)  # apply on the mask
        opening = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel)  # worked well

        if saveVideo:
            out.write(frame)

        # cv2.imshow("Result", result)
        cv2.imshow("Opening", opening)

        # exit loop
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()  # stop using camera
    cv2.destroyAllWindows()


"""
Edge Detect on video capture.
"""


def WebCamEdgeDetection(videoSourceNum: int, saveVideo: bool):

    capture = cv2.VideoCapture(videoSourceNum)

    # for saving video feed
    if saveVideo:
        fourcc = cv2.VideoWriter_fourcc(*'XVID')
        out = cv2.VideoWriter('outputVid.avi', fourcc, 20.0, (640, 480))

    while True:
        ret, frame = capture.read()

        cannyEdges = cv2.Canny(frame, 100, 200)

        if saveVideo:
            out.write(frame)

        cv2.imshow("Canny", cannyEdges)

        # exit loop
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()  # stop using camera
    cv2.destroyAllWindows()


###############################################################################
# Main Function
###############################################################################

def Main():

    # img loading
    img_path = "EverythingWillBeOk.jpg"
    img2_path = "bookpage.jpg"
    # print(f"\nPlotting Image at {img_path} in grayscale")
    # ReadImageGray(path=img_path)
    # ImageThresholding(img2_path)

    # live video
    # print(f"\nShowing live video feed.\n\nPress Q to exit.")
    # ReadWebCamFeed(0, False)
    WebCamColorFiltering(0, False)
    # WebCamEdgeDetection(0, False)


if __name__ == "__main__":
    Main()

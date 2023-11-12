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


###############################################################################
# Main Function
###############################################################################

def Main():

    # img loading
    # img_path = "EverythingWillBeOk.jpg"
    # print(f"\nPlotting Image at {img_path} in grayscale")
    # ReadImageGray(path=img_path)

    # live video
    print(f"\nShowing live video feed.\n\nPress Q to exit.")
    ReadWebCamFeed(0, False)


if __name__ == "__main__":
    Main()

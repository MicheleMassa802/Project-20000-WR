import argparse
import socket
import json
import cv2
from Utils import GetLimits
import DetectInput as DI

# COMM CONSTANTS
HOST = "127.0.0.1"
PORT = 12345

# PROC CONSTANTS
SZ_CENTER = (320, 301)

###############################################################################
# File for sending bat data through a local server to the Unity Game 
# (after slight processing)
###############################################################################


"""
Function making use of DetectInput.py to send bat data across a local server
to Unity.

Args:
- videoSourceNum: index of the camera to use for video input
- righty: boolean keeping track of whether the batter is a righty or a lefty

"""


def ColorFilteringDataSender(videoSourceNum: int, righty: bool):

    bat_pos = None
    bat_speed = None
    bat_dir = None
    # data to send
    pixel_position = (0, 0)  # the position of the bat's center in pixel form in the (640, 480) frame
    swing = 0  # whether the player swung or not in this frame, default is 0

    # Create the socket and bind it
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)  # ipv4, tcp
    s.bind((HOST, PORT))

    s.listen(1)  # listen for a single connection (the game client)

    # wait for client and establish connection
    client_s, address = None, None

    while True:
        client_s, address = s.accept()
        print(f"Connection from client {address} has been established.")
        client_s.send(bytes("Connection with server established.", "utf-8"))

        if client_s != None:
            break

    # start the video analysis
    cap = cv2.VideoCapture(videoSourceNum)

    # manual thresholds based on testing
    lower_red, upper_red = GetLimits(DI.RED)

    try:
        while True:
            bat_data = {'pos': bat_pos, 'spd': bat_speed, 'dir': bat_dir}
            send_data = {'pixel_position': pixel_position, 'swing': swing}
            frame, mask, bat_data = DI.WebCamColorFilteringIteration(
                cap, DI.BatDataV2, bat_data, lower_red, upper_red, righty)

            # register updates & modify pos to be json serializeable
            bat_pos = bat_data['pos']
            bat_speed = bat_data['spd']
            bat_dir = bat_data['dir']

            if not (bat_pos is None):
                bat_data['pos'] = (int(bat_pos[0]), int(bat_pos[1]))

            # parse the data and send it
            _, swing = ParseBatData(bat_data)
            send_data['pixel_position'] = bat_data['pos']
            send_data['swing'] = swing

            client_s.send(json.dumps(send_data).encode())

            cv2.imshow("OG", frame)
            # cv2.imshow("Mask", mask)

            # exit loop on q key-press
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

    except socket.error:
        # game stops => close server down
        print("Client disconnected")

    finally:
        cap.release()  # stop using camera
        cv2.destroyAllWindows()

        # close the connection
        s.close()


"""
Parses bat data to return the scaled distance from the center of the strike
zone and determines whether there was a swing or not.

Args:
- bat_data: a dictionary of form {'pos': (x,y), 'spd': s, 'dir': d}

Return: (in the form of a tuple)
- Scaled distance from strikezone center in the form of a tuple (x_dist, y_dist)
- 0 for no swing, 1 for swing
"""
def ParseBatData(bat_data):
    pos = bat_data['pos']
    speed = bat_data['spd']

    # for the first iteration
    if pos is None or speed is None:
        return ((0,0), 0)

    scaled_dist_x = int((pos[0] - SZ_CENTER[0]) / 5)
    scaled_dist_y = int((pos[1] - SZ_CENTER[1]) / 5)

    swing = 1 if speed > 80 else 0

    return ((scaled_dist_x, scaled_dist_y), swing)


###############################################################################
# Rest
###############################################################################


if __name__ == "__main__":

    # get arg
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--isRighty",
        type=str,  # to parse multiple values
        required=True,
        help="A bool stating wether the batter is a righty or lefty \n" +
        "(use true, T, t, Y, y, yes, or 1 for a positive value)"
    )
    args = parser.parse_args()
    isRighty = args.isRighty.lower() in ['true', '1', 't', 'y', 'yes']

    # live video
    print(f"\nShowing live video feed.\n\nPress Q to exit.")
    ColorFilteringDataSender(0, isRighty)

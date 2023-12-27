import socket
import json
import cv2
from Utils import GetLimits
import DetectInput as DI

# CONSTANTS
HOST = "127.0.0.1"
PORT = 12345


"""
Function making use of DetectInput.py to send bat data across a local server
to Unity.
"""


def ColorFilteringDataSender(videoSourceNum: int):

    bat_pos = None
    bat_speed = None
    bat_dir = None

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
            frame, mask, bat_data = DI.WebCamColorFilteringIteration(
                cap, bat_data, lower_red, upper_red)

            # register updates & modify pos to be json serializeable
            bat_pos = bat_data['pos']
            bat_speed = bat_data['spd']
            bat_dir = bat_data['dir']

            if not (bat_pos is None):
                bat_data['pos'] = (int(bat_pos[0]), int(bat_pos[1]))

            client_s.send(json.dumps(bat_data).encode())

            cv2.imshow("OG", frame)
            cv2.imshow("Mask", mask)

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


if __name__ == "__main__":
    # live video
    print(f"\nShowing live video feed.\n\nPress Q to exit.")
    ColorFilteringDataSender(0)

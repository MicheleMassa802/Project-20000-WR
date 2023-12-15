import socket
import json
from SendInput import PORT

# CONSTANTS
BUFFER_SIZE = 1024

"""
Test data receiver that simply spits out all the data it gets from
its connection with SendInput.py
"""


def DataReceiver():
    # Create the socket
    s = socket.socket()

    # connect to the server on local pc
    s.connect((socket.gethostname(), PORT))
    # .gethostname only used at it will run in the same machine

    welcome_msg = s.recv(BUFFER_SIZE)
    print(welcome_msg.decode("utf-8"))

    # enter loop for receiving actual data
    while True:
        data = s.recv(BUFFER_SIZE)

        # once we stop getting data, we stop listenning and disconnect
        if not data:
            break

        # decode and print the received data
        bat_data = json.loads(data.decode())
        print("\n" + str(bat_data))

    # close the connection
    s.close()


if __name__ == "__main__":
    DataReceiver()

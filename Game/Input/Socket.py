import socket
import json
import DetectInput as DI

# set up socket server to send data to unity
server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

server.bind(('localhost', 12345))
server.listen(1)

while True:
    conn, address = server.accept()

    try:
        # call for bat detection
        DI.Main()
        batPosition = (0, 0, 0)

        conn.sendall(json.dumps(batPosition).encode('utf-8'))

    finally:
        # end connection
        conn.close()

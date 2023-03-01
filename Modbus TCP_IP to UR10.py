import socket
import struct

TCP_IP = '192.168.0.128'
TCP_PORT = 502
BUFFER_SIZE = 39
# 建立一個socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# 主動去連線本機IP和埠號為502的程序，localhost等效於127.0.0.1，也就是去連線本機埠為502的程序
sock.connect((TCP_IP, TCP_PORT))

try:
    print("\n,Switching plug on")
    # Modbus、資料進行編碼格式轉換
    req = struct.pack('12B', 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x01, 0x02, 0x00, 0x11,0x00,0x01)
    # 傳送資料
    sock.send(req)
    print("TX: (%s)" % req)
    # 接收服務端的反饋資料
    data = sock.recv(1024)
    print(b'Received from server: ' + data)
    
finally:
    print('\nCLOSING SOCKET')
    sock.close()

import socket
import cv2

#정수를 보내기 위한 함수
def sendInt(socket :socket.socket, data : int) -> int:
    #숫자 끝에 NULL문자를 보내 숫자의 끝을 나타낸다.
    return socket.send((str(data) + '\0').encode())

def sendImage(socket :socket.socket,img):
    bImgData = cv2.imencode('.jpg',img)[1].tostring()
    sendInt(socket, len(bImgData))
    socket.send(bImgData)
    
def recvInt(sock : socket.socket) -> int:
	bData = bytearray()
	while True:
		aByte = sock.recv(1)
		if aByte[0] == 0:
			break
		bData.append(aByte[0])

	return int(bData.decode())

def recvImg(sock : socket.socket):
	ImgSize = recvInt(sock)
	bData = bytearray()
	
	while len(bData) < ImgSize:
		bData += bytearray(sock.recv(ImgSize - len(bData)))

	bData = bytes(bData)	#convert bytearry to bytes

	Img = np.frombuffer(bData,dtype=np.dtype(np.uint8))
	Img = Img.reshape(ImgSize,1)
	return cv2.imdecode(Img,cv2.IMREAD_COLOR)

server_socket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)

server_socket.setsockopt(socket.SOL_SOCKET,socket.SO_REUSEADDR,1)

server_socket.bind(('',9999))

server_socket.listen()

print('waiting...')

client_socket,addr = server_socket.accept()

print('client is accpeted!')

img = cv2.imread('안경쓴 고양이.jpg')

sendImage(client_socket,img)

client_socket.close()
server_socket.close()

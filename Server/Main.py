import socket
import cv2

#정수를 보내기 위한 함수
def sendInt(socket :socket.socket,data : int) -> int:
    #숫자 끝에 NULL문자를 보내 숫자의 끝을 나타낸다.
    return socket.send((str(data) + '\0').encode())

def sendImage(socket :socket.socket,img):
    sendInt(client_socket,len(img[0]))  #width 크기 전송
    sendInt(client_socket,len(img))     #height 크기 전송

    client_socket.send(img)             #사진 데이터 전송

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
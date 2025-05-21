import requests

url = "http://192.168.16.243:5000/transcribe"
headers = {"Content-Type": "audio/wav"}
with open("hello.wav", "rb") as f:
    resp = requests.post(url, headers=headers, data=f.read())

print(resp.status_code, resp.json())

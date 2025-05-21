import requests

url = "https://vasu14devagarwal--whisper-fastapi-fastapi-app.modal.run/transcribe"
headers = {"Content-Type": "audio/wav"}
with open("hello.wav", "rb") as f:
    resp = requests.post(url, headers=headers, data=f.read())

print(resp.status_code, resp.json())

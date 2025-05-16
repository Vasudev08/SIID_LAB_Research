import requests

url = "https://siidlabresearch-production.up.railway.app/transcribe"
headers = {"Content-Type": "audio/wav"}
with open("hello.wav", "rb") as f:
    resp = requests.post(url, headers=headers, data=f.read())

print(resp.status_code, resp.json())

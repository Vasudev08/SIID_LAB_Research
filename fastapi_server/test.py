import time
import requests

url = "https://siidlabresearch-production.up.railway.app/transcribe"
headers = {"Content-Type": "audio/wav"}

# Start the timer
start = time.perf_counter()

with open("hello.wav", "rb") as f:
    audio_bytes = f.read()

# Send the request
resp = requests.post(url, headers=headers, data=audio_bytes)

# Stop the timer
elapsed = time.perf_counter() - start

# Output timing + response
print(f"Request completed in {elapsed:.3f} seconds")
print(resp.status_code, resp.json())

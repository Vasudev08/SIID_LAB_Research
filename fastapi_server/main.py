from fastapi import FastAPI, Request, HTTPException
from transformers import pipeline
import io
import soundfile as sf
import numpy as np
import torch
import time
import logging
import sys

# -----------------------------------
# Logger Configuration
# -----------------------------------
logger = logging.getLogger("asr_api")
logger.setLevel(logging.INFO)

# Log format
formatter = logging.Formatter("%(asctime)s [%(levelname)s] %(message)s")

# Live Console Handler
console_handler = logging.StreamHandler(sys.stdout)
console_handler.setFormatter(formatter)
logger.addHandler(console_handler)

# File Handler
file_handler = logging.FileHandler("asr_requests.log")
file_handler.setFormatter(formatter)
logger.addHandler(file_handler)

# -----------------------------------
# FastAPI App Setup
# -----------------------------------
app = FastAPI()

# Load Whisper model
device = 0 if torch.cuda.is_available() else -1
asr_pipeline = pipeline(
    "automatic-speech-recognition",
    model="openai/whisper-small",
    device=device,
    chunk_length_s=30
)

# -----------------------------------
# Middleware: Log Each Request
# -----------------------------------
@app.middleware("http")
async def log_requests(request: Request, call_next):
    start_time = time.time()
    try:
        response = await call_next(request)
        process_time = (time.time() - start_time) * 1000
        content_length = request.headers.get("content-length", "unknown")

        logger.info(
            f"{request.method} {request.url.path} | Size: {content_length} bytes | "
            f"Status: {response.status_code} | Time: {process_time:.2f} ms"
        )

        return response
    except Exception as e:
        logger.error(f"Unhandled Exception in request: {e}")
        raise e

# -----------------------------------
# Endpoint: Transcribe Audio
# -----------------------------------
@app.post("/transcribe")
async def transcribe(request: Request):
    try:
        data = await request.body()
        audio_np, sr = sf.read(io.BytesIO(data))

        if audio_np.ndim > 1:
            audio_np = np.mean(audio_np, axis=1)

        result = asr_pipeline({"array": audio_np, "sampling_rate": sr})

        logger.info(f"Response: {result['text'][:100]}")  # Log first 100 chars of transcript
        return {"text": result["text"]}

    except Exception as e:
        logger.exception("Transcription failed")
        raise HTTPException(status_code=500, detail=str(e))

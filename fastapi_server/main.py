from fastapi import FastAPI, Request, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from transformers import pipeline
import io, soundfile as sf

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],            # or lock this down to specific domains
    allow_methods=["POST"],
    allow_headers=["*"],
)

asr_pipeline = pipeline("automatic-speech-recognition", model="openai/whisper-small")

@app.post("/transcribe")
async def transcribe(request: Request):
    data = await request.body()

    try:
        # data = await audio.read()
        audio_np, sr = sf.read(io.BytesIO(data))

        if audio_np.ndim > 1:
            audio_np = audio_np.mean(axis=1)

        result = asr_pipeline({"array": audio_np, "sampling_rate": sr})

        return {"text": result["text"]}


    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

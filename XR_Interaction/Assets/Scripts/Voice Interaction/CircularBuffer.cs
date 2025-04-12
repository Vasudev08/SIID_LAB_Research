using System;
using UnityEngine;

public class CircularBuffer
{
    private float[] buffer;
    private int capacity;
    private int size;
    private int start;
    private int end; // tail of the buffer. Points to the next index after the last element
   

    public CircularBuffer(int capacity)
    {
        this.capacity = capacity;
        this.buffer = new float[capacity];
        this.size = 0;
        this.start = 0;
        this.end = 0;
        
    }

    public void Write(float[] data, int length)
    {
        for (int i = 0; i < length; i ++)
        {
            // Writes the next available spot with data.
            // If capacity is full this implementation will overwrite the oldest element in the buffer with the new data.

            buffer[end] = data[i];
            end = (end + 1) % capacity;


            if (size < capacity)
            {
                // If there is enough space in the buffer just increase the size.
                size += 1;
            }
            else
            {
                // If we ran out of space in the buffer we need to overwrite data.
                // So increase the head or start index of the array to the next index "above" it which is the next oldest element.
                start = (start + 1) % capacity;

            }
        }
    }


    // Get the data in a continuous array
    public float[] GetData()
    {
        float[] data = new float[size];

        if (start < end)
        {
            Array.Copy(buffer, start, data, 0, size);
        }
        else // Buffer is full so we have looped around
        {
            int length_of_first_section = capacity - start;
            Array.Copy(buffer, start, data, 0, length_of_first_section);
            Array.Copy(buffer, 0, data, length_of_first_section, end);

        }

        return data;
    }

    // Clears the buffer resetting pointers to 0.
    public void Clear()
    {
        size = 0;
        start = 0;
        end = 0;
    }
}
using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    // Convert AudioClip to WAV file format
    public static byte[] FromAudioClip(AudioClip audioClip)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        // Write the file header
        writer.Write("RIFF".ToCharArray());
        writer.Write(0); // placeholder for file size
        writer.Write("WAVE".ToCharArray());

        // Write the format chunk
        writer.Write("fmt ".ToCharArray());
        writer.Write(16); // PCM chunk size
        writer.Write((short)1); // audio format (PCM)
        writer.Write((short)audioClip.channels);
        writer.Write(audioClip.frequency);
        writer.Write(audioClip.frequency * audioClip.channels * 2); // byte rate
        writer.Write((short)(audioClip.channels * 2)); // block align
        writer.Write((short)16); // bits per sample

        // Write the data chunk
        writer.Write("data".ToCharArray());
        writer.Write(0); // placeholder for data chunk size

        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        foreach (var sample in samples)
        {
            short s = (short)(sample * short.MaxValue);
            writer.Write(s);
        }

        // Fill in the size information
        writer.Seek(4, SeekOrigin.Begin);
        writer.Write((int)(stream.Length - 8));

        writer.Seek(40, SeekOrigin.Begin);
        writer.Write((int)(stream.Length - 44));

        writer.Flush();
        return stream.ToArray();
    }

    // Convert WAV file format to AudioClip (not used in this context but useful for future)
    public static AudioClip ToAudioClip(byte[] wavData)
    {
        using (MemoryStream stream = new MemoryStream(wavData))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            reader.ReadBytes(4); // ChunkID
            reader.ReadInt32(); // ChunkSize
            reader.ReadBytes(4); // Format
            reader.ReadBytes(4); // Subchunk1ID
            reader.ReadInt32(); // Subchunk1Size
            reader.ReadInt16(); // AudioFormat
            int channels = reader.ReadInt16(); // NumChannels
            int sampleRate = reader.ReadInt32(); // SampleRate
            reader.ReadInt32(); // ByteRate
            reader.ReadInt16(); // BlockAlign
            int bitsPerSample = reader.ReadInt16(); // BitsPerSample

            reader.ReadBytes(4); // Subchunk2ID
            int dataSize = reader.ReadInt32(); // Subchunk2Size

            float[] data = new float[dataSize / (bitsPerSample / 8)];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = reader.ReadInt16() / (float)short.MaxValue;
            }

            AudioClip audioClip = AudioClip.Create("WavClip", data.Length, channels, sampleRate, false);
            audioClip.SetData(data, 0);
            return audioClip;
        }
    }
}

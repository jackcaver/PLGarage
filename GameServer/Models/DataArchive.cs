using System.Buffers.Binary;
using System.IO;

namespace GameServer.Models;

public class DataArchive
{
    public byte Version { get; set; }
    public byte[] CreationData { get; set; } //trk or sav
    public byte[] Metadata { get; set; } //cmd or cm2
    public byte[] NavData { get; set; } //nav
    public byte[] HudData { get; set; } //hud

    public bool TryParse(Stream data, bool isVita)
    {
        var header = new byte[4];
        data.ReadExactly(header);

        if (!header[0 .. 3].Equals("CTH"u8.ToArray()))
            return false;
        
        Version = header[3];

        var intBuffer1 = new byte[4];
        var intBuffer2 = new byte[4];
        int creationData1Size;
        int metadataSize;
        
        switch (Version)
        {
            case 1:
                data.ReadExactly(intBuffer1);
                data.ReadExactly(intBuffer2);

                creationData1Size = isVita ? BinaryPrimitives.ReadInt32LittleEndian(intBuffer1) 
                    : BinaryPrimitives.ReadInt32BigEndian(intBuffer1);
                metadataSize = isVita ? BinaryPrimitives.ReadInt32LittleEndian(intBuffer2) 
                    : BinaryPrimitives.ReadInt32BigEndian(intBuffer2);

                CreationData = new byte[creationData1Size];
                Metadata = new byte[metadataSize];
                
                data.ReadExactly(CreationData);
                data.ReadExactly(Metadata);
                break;
            
            case 4 when !isVita:
                data.ReadExactly(intBuffer1);
                data.ReadExactly(intBuffer2);

                creationData1Size = BinaryPrimitives.ReadInt32BigEndian(intBuffer1);
                metadataSize = BinaryPrimitives.ReadInt32BigEndian(intBuffer2);
                
                data.ReadExactly(intBuffer1);
                data.ReadExactly(intBuffer2);

                var navDataSize = BinaryPrimitives.ReadInt32BigEndian(intBuffer1);
                var hudDataSize = BinaryPrimitives.ReadInt32BigEndian(intBuffer2);
            
                CreationData = new byte[creationData1Size];
                Metadata = new byte[metadataSize];
                NavData = new byte[navDataSize];
                HudData = new byte[hudDataSize];
                
                data.ReadExactly(CreationData);
                data.ReadExactly(Metadata);
                data.ReadExactly(NavData);
                data.ReadExactly(HudData);
                break;
            
            default:
                return false;
        }
        
        return true;
    }

    public byte[] ToArray(bool isVita)
    {
        using var stream = new MemoryStream();

        stream.Write("CTH"u8.ToArray());
        stream.WriteByte(Version);

        var intBuffer1 = new byte[4];
        var intBuffer2 = new byte[4];
        
        switch (Version)
        {
            case 1:
                if (isVita)
                    BinaryPrimitives.WriteInt32LittleEndian(intBuffer1, CreationData.Length);
                else
                    BinaryPrimitives.WriteInt32BigEndian(intBuffer1, CreationData.Length);
                
                if (isVita)
                    BinaryPrimitives.WriteInt32LittleEndian(intBuffer2, Metadata.Length);
                else
                    BinaryPrimitives.WriteInt32BigEndian(intBuffer2, Metadata.Length);
                
                stream.Write(intBuffer1);
                stream.Write(intBuffer2);
                
                stream.Write(CreationData);
                stream.Write(Metadata);
                break;
            
            case 4 when !isVita:
                BinaryPrimitives.WriteInt32BigEndian(intBuffer1, CreationData.Length);
                BinaryPrimitives.WriteInt32BigEndian(intBuffer2, Metadata.Length);
                
                stream.Write(intBuffer1);
                stream.Write(intBuffer2);
                
                BinaryPrimitives.WriteInt32BigEndian(intBuffer1, NavData.Length);
                BinaryPrimitives.WriteInt32BigEndian(intBuffer2, HudData.Length);
                
                stream.Write(intBuffer1);
                stream.Write(intBuffer2);
                
                stream.Write(CreationData);
                stream.Write(Metadata);
                stream.Write(NavData);
                stream.Write(HudData);
                break;
            
            default:
                return [];
        }
        
        return stream.ToArray();
    }
}
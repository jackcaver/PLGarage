using System.Buffers.Binary;
using System.IO;

namespace GameServer.Models;

public class DataArchive
{//TODO: figure out file names
    public byte Version { get; set; }
    public byte[] UnkData1 { get; set; }
    public byte[] UnkData2 { get; set; }
    public byte[] UnkData3 { get; set; }
    public byte[] UnkData4 { get; set; }

    public bool TryParse(Stream data, bool isVita)
    {
        var header = new byte[4];
        data.ReadExactly(header);

        if (!header[0 .. 3].Equals("CTH"u8.ToArray()))
            return false;
        
        Version = header[3];

        var intBuffer1 = new byte[4];
        var intBuffer2 = new byte[4];
        int unkData1Size;
        int unkData2Size;
        
        switch (Version)
        {
            case 1:
                data.ReadExactly(intBuffer1);
                data.ReadExactly(intBuffer2);

                unkData1Size = isVita ? BinaryPrimitives.ReadInt32LittleEndian(intBuffer1) 
                    : BinaryPrimitives.ReadInt32BigEndian(intBuffer1);
                unkData2Size = isVita ? BinaryPrimitives.ReadInt32LittleEndian(intBuffer2) 
                    : BinaryPrimitives.ReadInt32BigEndian(intBuffer2);

                UnkData1 = new byte[unkData1Size];
                UnkData2 = new byte[unkData2Size];
                
                data.ReadExactly(UnkData1);
                data.ReadExactly(UnkData2);
                break;
            
            case 4 when !isVita:
                data.ReadExactly(intBuffer1);
                data.ReadExactly(intBuffer2);

                unkData1Size = BinaryPrimitives.ReadInt32BigEndian(intBuffer1);
                unkData2Size = BinaryPrimitives.ReadInt32BigEndian(intBuffer2);
                
                data.ReadExactly(intBuffer1);
                data.ReadExactly(intBuffer2);

                var unkData3Size = BinaryPrimitives.ReadInt32BigEndian(intBuffer1);
                var unkData4Size = BinaryPrimitives.ReadInt32BigEndian(intBuffer2);
            
                UnkData1 = new byte[unkData1Size];
                UnkData2 = new byte[unkData2Size];
                UnkData3 = new byte[unkData3Size];
                UnkData4 = new byte[unkData4Size];
                
                data.ReadExactly(UnkData1);
                data.ReadExactly(UnkData2);
                data.ReadExactly(UnkData3);
                data.ReadExactly(UnkData4);
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
                    BinaryPrimitives.WriteInt32LittleEndian(intBuffer1, UnkData1.Length);
                else
                    BinaryPrimitives.WriteInt32BigEndian(intBuffer1, UnkData1.Length);
                
                if (isVita)
                    BinaryPrimitives.WriteInt32LittleEndian(intBuffer2, UnkData2.Length);
                else
                    BinaryPrimitives.WriteInt32BigEndian(intBuffer2, UnkData2.Length);
                
                stream.Write(intBuffer1);
                stream.Write(intBuffer2);
                
                stream.Write(UnkData1);
                stream.Write(UnkData2);
                break;
            
            case 4 when !isVita:
                BinaryPrimitives.WriteInt32BigEndian(intBuffer1, UnkData1.Length);
                BinaryPrimitives.WriteInt32BigEndian(intBuffer2, UnkData2.Length);
                
                stream.Write(intBuffer1);
                stream.Write(intBuffer2);
                
                BinaryPrimitives.WriteInt32BigEndian(intBuffer1, UnkData3.Length);
                BinaryPrimitives.WriteInt32BigEndian(intBuffer2, UnkData4.Length);
                
                stream.Write(intBuffer1);
                stream.Write(intBuffer2);
                
                stream.Write(UnkData1);
                stream.Write(UnkData2);
                stream.Write(UnkData3);
                stream.Write(UnkData4);
                break;
            
            default:
                return [];
        }
        
        return stream.ToArray();
    }
}
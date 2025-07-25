﻿using System;
using System.Collections.Generic;
using System.Threading;
using IEC60870.Enum;

namespace IEC60870Driver
{
    public class BlockReader
    {
        public TypeId TypeId { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public Dictionary<int, object> Buffer { get; set; }

        public BlockReader()
        {
            Buffer = new Dictionary<int, object>();
        }

        public static List<BlockReader> Initialize(string blockSetting)
        {
            var result = new List<BlockReader>();
            try
            {
                // Format: "TypeId-From-To" (e.g., "M_SP_NA_1-1-100")
                var parts = blockSetting.Split('-');
                if (parts.Length != 3) return result;

                if (!Enum.TryParse<TypeId>(parts[0], out TypeId typeId))
                    return result;

                if (!int.TryParse(parts[1], out int from) || !int.TryParse(parts[2], out int to))
                    return result;

                result.Add(new BlockReader
                {
                    TypeId = typeId,
                    From = from,
                    To = to
                });
            }
            catch { }
            return result;
        }

        public bool Contains(int ioa)
        {
            return ioa >= From && ioa <= To;
        }

        public void ReadBlock(ClientAdapter clientAdapter, int commonAddress)
        {
            try
            {
                // Send general interrogation for this type range
                clientAdapter.Client.SendInterrogation(commonAddress);

                // Wait a bit for response
                Thread.Sleep(1000);

                // Get values from client buffer
                for (int ioa = From; ioa <= To; ioa++)
                {
                    if (clientAdapter.Client.GetValue(ioa, out object value))
                    {
                        Buffer[ioa] = value;
                    }
                }
            }
            catch { }
        }

        public string GetValue(int ioa)
        {
            if (Buffer.ContainsKey(ioa))
            {
                var value = Buffer[ioa];
                return value?.ToString() ?? "0";
            }
            return "0";
        }
    }
}
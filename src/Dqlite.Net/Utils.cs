﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Dqlite.Net
{
    internal static class Utils
    {
        public static ulong PadWord(ulong value)
        {
            if (value % 8 != 0)
            {
                value += (8 - (value % 8));
            }
            return value;
        }

        public static int PadWord(int value)
        {
            if(value == 0)
            {
                return 8;
            }
            else if (value % 8 != 0)
            {
                value += (8 - (value % 8));
            }
            return value;
        }

        public static bool TryParseEndPoint(ReadOnlySpan<char> value, out EndPoint endpoint)
        {
            if(!value.IsEmpty && value[0] == '/')
            {
                endpoint = new UnixDomainSocketEndPoint(value.ToString());
                return true;
            }

            var index = value.IndexOf(':');
            if(index == -1 || !int.TryParse(value.Slice(index+1), out var port)){
                endpoint = null;
                return false;
            }

            endpoint = new DnsEndPoint(value.Slice(0, index).ToString(), port, AddressFamily.InterNetwork);
            return true;            
        }

        public static bool TryParseAddress(ReadOnlySpan<char> value, out string address, out int port )
        {
            var index = value.IndexOf(':');
            if(index == -1 || !int.TryParse(value.Slice(index+1), out port)){
                address = null;
                port = 0;
                return false;
            }

            address = value.Slice(0, index).ToString();
            return true;            
        }

        public static int GetSize(DqliteParameter[] parameters)
        {
            var headerBits = 8 + parameters.Length * 8;
            var padBits = 0;
            if(headerBits % 64 != 0)
            {
                padBits = 64 - (headerBits % 64);
            }

            var length = (headerBits + padBits) / 64 * 8;
            foreach (var parameter in parameters)
            {
                length += PadWord(parameter.Size);
            }

            return length;
        }

        public static string ParseSql(string sqlText, out string[] parameterNames)
        {
            int IndexOfWhiteSpace(ReadOnlySpan<char> sql, int startIndex = 0)
            {
                var index = sql.Length;
                for(;startIndex < sql.Length; ++startIndex)
                {
                    if(!Char.IsLetterOrDigit(sql[startIndex]))
                    {
                        index = startIndex;
                        break;
                    }
                }
                return index;
            }

            if(sqlText == null)
            {
                parameterNames = new string[0];
                return null;
            }
            
            var sql = new Span<char>(sqlText.ToCharArray());
            var names = new List<string>();
            for(int i = 0, j = 0, y = 0; i < sql.Length; ++i)
            {
                if(sql[i] == '@')
                {
                    j = IndexOfWhiteSpace(sql, i+1);
                    
                    names.Add(sql.Slice(i, j-i).ToString());
                    sql[i++] = '?';
                    for(y = i;j< sql.Length; ++j)
                    {
                        sql[y++] = sql[j];
                    }
                    sql = sql.Slice(0, y);
                }
            }
            parameterNames = names.ToArray();
            return sql.ToString();
        }
    }
}    

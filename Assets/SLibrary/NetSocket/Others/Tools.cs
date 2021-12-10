using System;
using UnityEngine;
//using Message;

namespace SLibrary.NetSocket.Others
{

    public class Tools
    {
        /// <summary>
        /// 分割字符串到int数组中
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static int[] splitStringToIntArray(string src, char sign = '+')
        {
            if (String.IsNullOrEmpty(src))
            {
                return new int[3] {0, 0, 0};
            }
            else
            {
                string[] strs = src.Split(sign);
                int[] ret = new int[strs.Length];
                for (int i = 0; i < strs.Length; i++)
                {
                    if (!int.TryParse(strs[i], out ret[i]))
                    {
                        Debug.Log("字符串转int出错!");
                        continue;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// 将byte数组转换为string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string bytesToString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static byte[] stringToBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
    }
}
using System;

namespace SLibrary.NetSocket.Others
{

    /// <summary>
    /// 时间戳工具类
    /// </summary>
    public class TimeUtils
    {
        /// <summary>
        /// 当前时间的字符串表示
        /// </summary>
        /// <param name="utc"></param>
        /// <returns></returns>
        public static string current(bool utc = false)
        {
            if (utc)
                return DateTime.UtcNow.ToString();
            return DateTime.Now.ToString();
        }

        /// <summary>
        /// Unix时间戳(毫秒)
        /// 主线程
        /// </summary>
        /// <param name="utc"></param>
        /// <returns></returns>
        public static Int64 currentMilliseconds(bool utc = false)
        {
            /*
            if (GameManager.Singleton == null)
                return 0;
            if (GameManager.Singleton.IsDebugMode)
            {
                if (utc)
                    return (Int64)(DateTime.Now - mEpochUtc).TotalMilliseconds;
                return (Int64)(DateTime.Now - mEpochLocal).TotalMilliseconds;
            }
             */

            //if( mOffsetTime == 0 )
            //{
            return (long) (UnityEngine.Time.realtimeSinceStartup * 1000) - mClientLocalTime + mServerLoginTime;
            //}

            //return currentMillisecondsOnThread();
        }

        /// <summary>
        /// 支线程获取时间
        /// </summary>
        /// <param name="utc"></param>
        /// <returns></returns>
        public static Int64 currentMillisecondsOnThread(bool utc = false)
        {
            if (mOffsetTime == 0)
                return 0;
            return DateTime.Now.Ticks / 10000 + mOffsetTime;
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        public static DateTime Now()
        {
            return javaTimeToCSharpTime(currentMilliseconds());
        }

        /// <summary>
        /// 传入一个时间错返回一个DateTime，可单独取得年月日时分秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime OnePointTime(long time)
        {
            DateTime dataTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime startTime = dataTime.AddMilliseconds(time).ToLocalTime();
            return startTime;
        }

        /// <summary>
        /// Unix时间戳（秒）
        /// </summary>
        /// <param name="utc"></param>
        /// <returns></returns>
        public static Int32 currentSeconds(bool utc = false)
        {
            return (Int32) (currentMilliseconds(utc) / 1000);
        }

        /// <summary>
        /// TODO
        /// 设置客户端与服务器的时间差（同步时间时调用）
        /// </summary>
        /// <param name="serverTime"></param>
        public static void setServerTime(Int64 serverTime)
        {
            mServerLoginTime = serverTime;
            mClientLocalTime = (long) (UnityEngine.Time.realtimeSinceStartup * 1000);
            mOffsetTime = serverTime - (DateTime.Now.Ticks / 10000);
        }

        /// <summary>
        /// 毫秒转秒
        /// </summary>
        /// <param name="miniseconds">毫秒</param>
        /// <returns></returns>
        public static long MillisecondsToSeconds(long miniseconds)
        {
            return miniseconds / 1000;
        }

        //秒钟转分钟
        public static long SecondsToMin(long seconds)
        {
            return seconds / 60;
        }

        //秒钟转小时
        public static long SecondsToHour(long seconds)
        {
            return seconds / 3600;
        }

        /// <summary>
        /// 分钟转小时
        /// </summary>
        public static long MinToHour(long min)
        {
            return min / 60;
        }

        /// <summary>
        /// 分钟转天数
        /// </summary>
        public static long MinToDay(long min)
        {
            long hour = MinToHour(min);
            return hour / 24;
        }

        /// <summary>
        /// 分钟转月数
        /// </summary>
        public static long MinToMonth(long min)
        {
            long month = MinToDay(min);
            return month / 30;
        }

        //public static bool DebugMode = false;
        private static Int64 mClientLocalTime = 0;

        private static Int64 mServerLoginTime = 0;
        private static DateTime mEpochLocal = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        private static DateTime mEpochUtc = TimeZone.CurrentTimeZone.ToUniversalTime(new DateTime(1970, 1, 1));
        private static Int64 mOffsetTime;

        private static long mPauseStartTime = 0;

        public static void onGamePause()
        {
            mPauseStartTime = TimeUtils.currentMilliseconds();
        }

        public static void onGameResume()
        {
            //long pausedTime = TimeUtils.currentMilliseconds() - mPauseStartTime;
            //CoolDownManager.onGameResume(pausedTime);
            //mPauseStartTime = 0;
        }

        public static int[] getTimeArr(string timeStr)
        {
            int[] timeArr = Tools.splitStringToIntArray(timeStr, '_');
            if (timeArr.Length < 3)
            {
                //Logger.err("推送时间格式不对" + timeStr);
                return null;
            }

            return timeArr;
        }

        public static DateTime getNotifyTime(string timeStr, int offsetDay)
        {
            int year = System.DateTime.Now.Year;
            int month = System.DateTime.Now.Month;
            int day = System.DateTime.Now.Day;
            int[] timeArr = getTimeArr(timeStr);
            DateTime dateTime = DateTime.Now;
            if (timeArr != null)
            {
                dateTime = new System.DateTime(year, month, day, timeArr[0], timeArr[1], timeArr[2]);
                dateTime = dateTime.AddDays(offsetDay);
            }

            return dateTime;
        }

        /// <summary>
        /// java时间转C#时间
        /// </summary>
        public static DateTime javaTimeToCSharpTime(long javaMiniSeconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(javaMiniSeconds).AddHours(8);
        }

        public static object TimeLock = new object();
    }
}
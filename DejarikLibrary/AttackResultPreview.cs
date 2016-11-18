using System;
using System.Collections.Generic;

namespace DejarikLibrary
{
    public static class AttackResultPreview
    {
        private static Dictionary<int, Dictionary<int, Dictionary<AttackResult, decimal>>> ResultAverages { get; set; }

        static  AttackResultPreview()
        {
            BuildResultAverages();
        }

        public static IDictionary<AttackResult, decimal> GetAttackResultPercentages(int attack, int defense)
        {
            if (!ResultAverages.ContainsKey(attack))
            {
                throw new ArgumentException("Attack rating invalid");
            }
            if (!ResultAverages[attack].ContainsKey(defense))
            {
                throw new ArgumentException("Defense rating invalid");
            }

            return ResultAverages[attack][defense];

        }

        public static decimal GetAttackResultPercentage(int attack, int defense, AttackResult attackResult)
        {
            if (!ResultAverages.ContainsKey(attack))
            {
                throw new ArgumentException("Attack rating invalid");
            }
            if (!ResultAverages[attack].ContainsKey(defense))
            {
                throw new ArgumentException("Defense rating invalid");
            }
            if (!ResultAverages[attack][defense].ContainsKey(attackResult))
            {
                throw new ArgumentException("Attack result invalid");
            }

            return ResultAverages[attack][defense][attackResult];
        }


        private static void BuildResultAverages()
        {
            ResultAverages = new Dictionary<int, Dictionary<int, Dictionary<AttackResult, decimal>>>();
            ResultAverages.Add(2, new Dictionary<int, Dictionary<AttackResult, decimal>>());
            ResultAverages[2].Add(2, new Dictionary<AttackResult, decimal>());
            ResultAverages[2][2].Add(AttackResult.Kill, 0.0269m);
            ResultAverages[2][2].Add(AttackResult.Push, 0.4171m);
            ResultAverages[2][2].Add(AttackResult.CounterPush, 0.5291m);
            ResultAverages[2][2].Add(AttackResult.CounterKill, 0.0269m);
            ResultAverages[2].Add(3, new Dictionary<AttackResult, decimal>());
            ResultAverages[2][3].Add(AttackResult.Kill, 0.0027m);
            ResultAverages[2][3].Add(AttackResult.Push, 0.1495m);
            ResultAverages[2][3].Add(AttackResult.CounterPush, 0.6261m);
            ResultAverages[2][3].Add(AttackResult.CounterKill, 0.2217m);
            ResultAverages[2].Add(4, new Dictionary<AttackResult, decimal>());
            ResultAverages[2][4].Add(AttackResult.Kill, 0.0001m);
            ResultAverages[2][4].Add(AttackResult.Push, 0.0356m);
            ResultAverages[2][4].Add(AttackResult.CounterPush, 0.4169m);
            ResultAverages[2][4].Add(AttackResult.CounterKill, 0.5473m);
            ResultAverages[2].Add(5, new Dictionary<AttackResult, decimal>());
            ResultAverages[2][5].Add(AttackResult.Kill, 0.0000m);
            ResultAverages[2][5].Add(AttackResult.Push, 0.0062m);
            ResultAverages[2][5].Add(AttackResult.CounterPush, 0.1852m);
            ResultAverages[2][5].Add(AttackResult.CounterKill, 0.8086m);
            ResultAverages[2].Add(6, new Dictionary<AttackResult, decimal>());
            ResultAverages[2][6].Add(AttackResult.Kill, 0m);
            ResultAverages[2][6].Add(AttackResult.Push, 0.0008m);
            ResultAverages[2][6].Add(AttackResult.CounterPush, 0.0603m);
            ResultAverages[2][6].Add(AttackResult.CounterKill, 0.9389m);
            ResultAverages[2].Add(7, new Dictionary<AttackResult, decimal>());
            ResultAverages[2][7].Add(AttackResult.Kill, 0m);
            ResultAverages[2][7].Add(AttackResult.Push, 0.0001m);
            ResultAverages[2][7].Add(AttackResult.CounterPush, 0.0147m);
            ResultAverages[2][7].Add(AttackResult.CounterKill, 0.9852m);
            ResultAverages[2].Add(8, new Dictionary<AttackResult, decimal>());
            ResultAverages[2][8].Add(AttackResult.Kill, 0m);
            ResultAverages[2][8].Add(AttackResult.Push, 0.0000m);
            ResultAverages[2][8].Add(AttackResult.CounterPush, 0.0029m);
            ResultAverages[2][8].Add(AttackResult.CounterKill, 0.9971m);
            ResultAverages.Add(3, new Dictionary<int, Dictionary<AttackResult, decimal>>());
            ResultAverages[3].Add(2, new Dictionary<AttackResult, decimal>());
            ResultAverages[3][2].Add(AttackResult.Kill, 0.2214m);
            ResultAverages[3][2].Add(AttackResult.Push, 0.5572m);
            ResultAverages[3][2].Add(AttackResult.CounterPush, 0.2187m);
            ResultAverages[3][2].Add(AttackResult.CounterKill, 0.0028m);
            ResultAverages[3].Add(3, new Dictionary<AttackResult, decimal>());
            ResultAverages[3][3].Add(AttackResult.Kill, 0.0609m);
            ResultAverages[3][3].Add(AttackResult.Push, 0.3925m);
            ResultAverages[3][3].Add(AttackResult.CounterPush, 0.4859m);
            ResultAverages[3][3].Add(AttackResult.CounterKill, 0.0607m);
            ResultAverages[3].Add(4, new Dictionary<AttackResult, decimal>());
            ResultAverages[3][4].Add(AttackResult.Kill, 0.0121m);
            ResultAverages[3][4].Add(AttackResult.Push, 0.1790m);
            ResultAverages[3][4].Add(AttackResult.CounterPush, 0.5508m);
            ResultAverages[3][4].Add(AttackResult.CounterKill, 0.2581m);
            ResultAverages[3].Add(5, new Dictionary<AttackResult, decimal>());
            ResultAverages[3][5].Add(AttackResult.Kill, 0.0018m);
            ResultAverages[3][5].Add(AttackResult.Push, 0.0592m);
            ResultAverages[3][5].Add(AttackResult.CounterPush, 0.3976m);
            ResultAverages[3][5].Add(AttackResult.CounterKill, 0.5415m);
            ResultAverages[3].Add(6, new Dictionary<AttackResult, decimal>());
            ResultAverages[3][6].Add(AttackResult.Kill, 0.0002m);
            ResultAverages[3][6].Add(AttackResult.Push, 0.0146m);
            ResultAverages[3][6].Add(AttackResult.CounterPush, 0.2054m);
            ResultAverages[3][6].Add(AttackResult.CounterKill, 0.7798m);
            ResultAverages[3].Add(7, new Dictionary<AttackResult, decimal>());
            ResultAverages[3][7].Add(AttackResult.Kill, 0.0000m);
            ResultAverages[3][7].Add(AttackResult.Push, 0.0028m);
            ResultAverages[3][7].Add(AttackResult.CounterPush, 0.0804m);
            ResultAverages[3][7].Add(AttackResult.CounterKill, 0.9167m);
            ResultAverages[3].Add(8, new Dictionary<AttackResult, decimal>());
            ResultAverages[3][8].Add(AttackResult.Kill, 0m);
            ResultAverages[3][8].Add(AttackResult.Push, 0.0005m);
            ResultAverages[3][8].Add(AttackResult.CounterPush, 0.0250m);
            ResultAverages[3][8].Add(AttackResult.CounterKill, 0.9746m);
            ResultAverages.Add(4, new Dictionary<int, Dictionary<AttackResult, decimal>>());
            ResultAverages[4].Add(2, new Dictionary<AttackResult, decimal>());
            ResultAverages[4][2].Add(AttackResult.Kill, 0.5458m);
            ResultAverages[4][2].Add(AttackResult.Push, 0.3940m);
            ResultAverages[4][2].Add(AttackResult.CounterPush, 0.0601m);
            ResultAverages[4][2].Add(AttackResult.CounterKill, 0.0002m);
            ResultAverages[4].Add(3, new Dictionary<AttackResult, decimal>());
            ResultAverages[4][3].Add(AttackResult.Kill, 0.2575m);
            ResultAverages[4][3].Add(AttackResult.Push, 0.4854m);
            ResultAverages[4][3].Add(AttackResult.CounterPush, 0.2451m);
            ResultAverages[4][3].Add(AttackResult.CounterKill, 0.0121m);
            ResultAverages[4].Add(4, new Dictionary<AttackResult, decimal>());
            ResultAverages[4][4].Add(AttackResult.Kill, 0.0903m);
            ResultAverages[4][4].Add(AttackResult.Push, 0.3691m);
            ResultAverages[4][4].Add(AttackResult.CounterPush, 0.4503m);
            ResultAverages[4][4].Add(AttackResult.CounterKill, 0.0904m);
            ResultAverages[4].Add(5, new Dictionary<AttackResult, decimal>());
            ResultAverages[4][5].Add(AttackResult.Kill, 0.0248m);
            ResultAverages[4][5].Add(AttackResult.Push, 0.1959m);
            ResultAverages[4][5].Add(AttackResult.CounterPush, 0.4971m);
            ResultAverages[4][5].Add(AttackResult.CounterKill, 0.2821m);
            ResultAverages[4].Add(6, new Dictionary<AttackResult, decimal>());
            ResultAverages[4][6].Add(AttackResult.Kill, 0.0053m);
            ResultAverages[4][6].Add(AttackResult.Push, 0.0780m);
            ResultAverages[4][6].Add(AttackResult.CounterPush, 0.3806m);
            ResultAverages[4][6].Add(AttackResult.CounterKill, 0.5362m);
            ResultAverages[4].Add(7, new Dictionary<AttackResult, decimal>());
            ResultAverages[4][7].Add(AttackResult.Kill, 0.0009m);
            ResultAverages[4][7].Add(AttackResult.Push, 0.0247m);
            ResultAverages[4][7].Add(AttackResult.CounterPush, 0.2168m);
            ResultAverages[4][7].Add(AttackResult.CounterKill, 0.7576m);
            ResultAverages[4].Add(8, new Dictionary<AttackResult, decimal>());
            ResultAverages[4][8].Add(AttackResult.Kill, 0.0001m);
            ResultAverages[4][8].Add(AttackResult.Push, 0.0061m);
            ResultAverages[4][8].Add(AttackResult.CounterPush, 0.0977m);
            ResultAverages[4][8].Add(AttackResult.CounterKill, 0.8961m);
            ResultAverages.Add(6, new Dictionary<int, Dictionary<AttackResult, decimal>>());
            ResultAverages[6].Add(2, new Dictionary<AttackResult, decimal>());
            ResultAverages[6][2].Add(AttackResult.Kill, 0.9395m);
            ResultAverages[6][2].Add(AttackResult.Push, 0.0587m);
            ResultAverages[6][2].Add(AttackResult.CounterPush, 0.0018m);
            ResultAverages[6][2].Add(AttackResult.CounterKill, 0m);
            ResultAverages[6].Add(3, new Dictionary<AttackResult, decimal>());
            ResultAverages[6][3].Add(AttackResult.Kill, 0.7802m);
            ResultAverages[6][3].Add(AttackResult.Push, 0.1949m);
            ResultAverages[6][3].Add(AttackResult.CounterPush, 0.0247m);
            ResultAverages[6][3].Add(AttackResult.CounterKill, 0.0002m);
            ResultAverages[6].Add(4, new Dictionary<AttackResult, decimal>());
            ResultAverages[6][4].Add(AttackResult.Kill, 0.5360m);
            ResultAverages[6][4].Add(AttackResult.Push, 0.3476m);
            ResultAverages[6][4].Add(AttackResult.CounterPush, 0.1110m);
            ResultAverages[6][4].Add(AttackResult.CounterKill, 0.0055m);
            ResultAverages[6].Add(5, new Dictionary<AttackResult, decimal>());
            ResultAverages[6][5].Add(AttackResult.Kill, 0.3013m);
            ResultAverages[6][5].Add(AttackResult.Push, 0.3997m);
            ResultAverages[6][5].Add(AttackResult.CounterPush, 0.2608m);
            ResultAverages[6][5].Add(AttackResult.CounterKill, 0.0382m);
            ResultAverages[6].Add(6, new Dictionary<AttackResult, decimal>());
            ResultAverages[6][6].Add(AttackResult.Kill, 0.1375m);
            ResultAverages[6][6].Add(AttackResult.Push, 0.3300m);
            ResultAverages[6][6].Add(AttackResult.CounterPush, 0.3952m);
            ResultAverages[6][6].Add(AttackResult.CounterKill, 0.1373m);
            ResultAverages[6].Add(7, new Dictionary<AttackResult, decimal>());
            ResultAverages[6][7].Add(AttackResult.Kill, 0.0524m);
            ResultAverages[6][7].Add(AttackResult.Push, 0.2073m);
            ResultAverages[6][7].Add(AttackResult.CounterPush, 0.4260m);
            ResultAverages[6][7].Add(AttackResult.CounterKill, 0.3143m);
            ResultAverages[6].Add(8, new Dictionary<AttackResult, decimal>());
            ResultAverages[6][8].Add(AttackResult.Kill, 0.0166m);
            ResultAverages[6][8].Add(AttackResult.Push, 0.1058m);
            ResultAverages[6][8].Add(AttackResult.CounterPush, 0.3471m);
            ResultAverages[6][8].Add(AttackResult.CounterKill, 0.5306m);
            ResultAverages.Add(7, new Dictionary<int, Dictionary<AttackResult, decimal>>());
            ResultAverages[7].Add(2, new Dictionary<AttackResult, decimal>());
            ResultAverages[7][2].Add(AttackResult.Kill, 0.9850m);
            ResultAverages[7][2].Add(AttackResult.Push, 0.0148m);
            ResultAverages[7][2].Add(AttackResult.CounterPush, 0.0002m);
            ResultAverages[7][2].Add(AttackResult.CounterKill, 0m);
            ResultAverages[7].Add(3, new Dictionary<AttackResult, decimal>());
            ResultAverages[7][3].Add(AttackResult.Kill, 0.9163m);
            ResultAverages[7][3].Add(AttackResult.Push, 0.0784m);
            ResultAverages[7][3].Add(AttackResult.CounterPush, 0.0053m);
            ResultAverages[7][3].Add(AttackResult.CounterKill, 0.0000m);
            ResultAverages[7].Add(4, new Dictionary<AttackResult, decimal>());
            ResultAverages[7][4].Add(AttackResult.Kill, 0.7578m);
            ResultAverages[7][4].Add(AttackResult.Push, 0.2039m);
            ResultAverages[7][4].Add(AttackResult.CounterPush, 0.0373m);
            ResultAverages[7][4].Add(AttackResult.CounterKill, 0.0010m);
            ResultAverages[7].Add(5, new Dictionary<AttackResult, decimal>());
            ResultAverages[7][5].Add(AttackResult.Kill, 0.5334m);
            ResultAverages[7][5].Add(AttackResult.Push, 0.3290m);
            ResultAverages[7][5].Add(AttackResult.CounterPush, 0.1272m);
            ResultAverages[7][5].Add(AttackResult.CounterKill, 0.0105m);
            ResultAverages[7].Add(6, new Dictionary<AttackResult, decimal>());
            ResultAverages[7][6].Add(AttackResult.Kill, 0.3152m);
            ResultAverages[7][6].Add(AttackResult.Push, 0.3700m);
            ResultAverages[7][6].Add(AttackResult.CounterPush, 0.2629m);
            ResultAverages[7][6].Add(AttackResult.CounterKill, 0.0520m);
            ResultAverages[7].Add(7, new Dictionary<AttackResult, decimal>());
            ResultAverages[7][7].Add(AttackResult.Kill, 0.1570m);
            ResultAverages[7][7].Add(AttackResult.Push, 0.3131m);
            ResultAverages[7][7].Add(AttackResult.CounterPush, 0.3744m);
            ResultAverages[7][7].Add(AttackResult.CounterKill, 0.1555m);
            ResultAverages[7].Add(8, new Dictionary<AttackResult, decimal>());
            ResultAverages[7][8].Add(AttackResult.Kill, 0.0653m);
            ResultAverages[7][8].Add(AttackResult.Push, 0.2093m);
            ResultAverages[7][8].Add(AttackResult.CounterPush, 0.3986m);
            ResultAverages[7][8].Add(AttackResult.CounterKill, 0.3269m);
            ResultAverages.Add(8, new Dictionary<int, Dictionary<AttackResult, decimal>>());
            ResultAverages[8].Add(2, new Dictionary<AttackResult, decimal>());
            ResultAverages[8][2].Add(AttackResult.Kill, 0.9970m);
            ResultAverages[8][2].Add(AttackResult.Push, 0.0029m);
            ResultAverages[8][2].Add(AttackResult.CounterPush, 0.0000m);
            ResultAverages[8][2].Add(AttackResult.CounterKill, 0m);
            ResultAverages[8].Add(3, new Dictionary<AttackResult, decimal>());
            ResultAverages[8][3].Add(AttackResult.Kill, 0.9747m);
            ResultAverages[8][3].Add(AttackResult.Push, 0.0244m);
            ResultAverages[8][3].Add(AttackResult.CounterPush, 0.0009m);
            ResultAverages[8][3].Add(AttackResult.CounterKill, 0m);
            ResultAverages[8].Add(4, new Dictionary<AttackResult, decimal>());
            ResultAverages[8][4].Add(AttackResult.Kill, 0.8966m);
            ResultAverages[8][4].Add(AttackResult.Push, 0.0930m);
            ResultAverages[8][4].Add(AttackResult.CounterPush, 0.0103m);
            ResultAverages[8][4].Add(AttackResult.CounterKill, 0.0001m);
            ResultAverages[8].Add(5, new Dictionary<AttackResult, decimal>());
            ResultAverages[8][5].Add(AttackResult.Kill, 0.7399m);
            ResultAverages[8][5].Add(AttackResult.Push, 0.2077m);
            ResultAverages[8][5].Add(AttackResult.CounterPush, 0.0500m);
            ResultAverages[8][5].Add(AttackResult.CounterKill, 0.0024m);
            ResultAverages[8].Add(6, new Dictionary<AttackResult, decimal>());
            ResultAverages[8][6].Add(AttackResult.Kill, 0.5309m);
            ResultAverages[8][6].Add(AttackResult.Push, 0.3128m);
            ResultAverages[8][6].Add(AttackResult.CounterPush, 0.1395m);
            ResultAverages[8][6].Add(AttackResult.CounterKill, 0.0169m);
            ResultAverages[8].Add(7, new Dictionary<AttackResult, decimal>());
            ResultAverages[8][7].Add(AttackResult.Kill, 0.3263m);
            ResultAverages[8][7].Add(AttackResult.Push, 0.3476m);
            ResultAverages[8][7].Add(AttackResult.CounterPush, 0.2606m);
            ResultAverages[8][7].Add(AttackResult.CounterKill, 0.0655m);
            ResultAverages[8].Add(8, new Dictionary<AttackResult, decimal>());
            ResultAverages[8][8].Add(AttackResult.Kill, 0.1721m);
            ResultAverages[8][8].Add(AttackResult.Push, 0.2995m);
            ResultAverages[8][8].Add(AttackResult.CounterPush, 0.3560m);
            ResultAverages[8][8].Add(AttackResult.CounterKill, 0.1724m);
        }
    }
}
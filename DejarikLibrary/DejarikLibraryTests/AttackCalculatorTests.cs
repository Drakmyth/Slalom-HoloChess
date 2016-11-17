using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DejarikLibrary;

namespace DejarikLibraryTests
{
    [TestClass]
    public class AttackCalculatorTests
    {
        [TestMethod]
        public void AttackCalculator_ReturnsAttackResultWithinMarginOfError()
        {
            const int iterations = 1000000;

            //<attack, defense, attackResult>
            Dictionary<int, Dictionary<int, Dictionary<AttackResult, decimal>>> resultAverages =
                new Dictionary<int, Dictionary<int, Dictionary<AttackResult, decimal>>>();

            AttackCalculator attackCalculator = new AttackCalculator();

            for (int a = 2; a < 9; a++)
            {
                //There are no attacks powers of 5
                if (a == 5)
                {
                    continue;
                }

                for (int d = 2; d < 9; d++)
                {
                    if (!resultAverages.ContainsKey(a))
                    {
                        resultAverages.Add(a, new Dictionary<int, Dictionary<AttackResult, decimal>>());
                    }
                    if (!resultAverages[a].ContainsKey(d))
                    {
                        resultAverages[a].Add(d, new Dictionary<AttackResult, decimal>());
                    }
                    resultAverages[a][d].Add(AttackResult.Kill, 0);
                    resultAverages[a][d].Add(AttackResult.Push, 0);
                    resultAverages[a][d].Add(AttackResult.CounterPush, 0);
                    resultAverages[a][d].Add(AttackResult.CounterKill, 0);
                }

                for (int d = 2; d < 9; d++)
                {
                    for (int i = 0; i < iterations; i++)
                    {

                        AttackResult result = attackCalculator.Calculate(a, d);
                        switch (result)
                        {
                            case AttackResult.Kill:
                                resultAverages[a][d][AttackResult.Kill]++;
                                break;
                            case AttackResult.Push:
                                resultAverages[a][d][AttackResult.Push]++;
                                break;
                            case AttackResult.CounterPush:
                                resultAverages[a][d][AttackResult.CounterPush]++;
                                break;
                            case AttackResult.CounterKill:
                                resultAverages[a][d][AttackResult.CounterKill]++;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    resultAverages[a][d][AttackResult.Kill] =
                        Math.Round(resultAverages[a][d][AttackResult.Kill]/iterations, 4);
                    resultAverages[a][d][AttackResult.Push] =
                        Math.Round(resultAverages[a][d][AttackResult.Push]/iterations, 4);
                    resultAverages[a][d][AttackResult.CounterPush] =
                        Math.Round(resultAverages[a][d][AttackResult.CounterPush]/iterations, 4);
                    resultAverages[a][d][AttackResult.CounterKill] =
                        Math.Round(resultAverages[a][d][AttackResult.CounterKill]/iterations, 4);
                }
            }


            System.Diagnostics.Debug.WriteLine(
                "Dictionary<int, Dictionary<int, Dictionary<AttackResult, decimal>>> resultAverages = new Dictionary<int, Dictionary<int, Dictionary<AttackResult, decimal>>>(); ");

            for (int a = 2; a < 9; a++)
            {
                //There are no attacks powers of 5
                if (a == 5)
                {
                    continue;
                }

                System.Diagnostics.Debug.WriteLine("resultAverages.Add(" + a +
                                  ", new Dictionary<int, Dictionary<AttackResult, decimal>>());");

                for (int d = 2; d < 9; d++)
                {
                    System.Diagnostics.Debug.WriteLine("resultAverages[" + a + "].Add(" + d +
                                      ", new Dictionary<AttackResult, decimal>()));");

                    System.Diagnostics.Debug.WriteLine("resultAverages[" + a + "][" + d + "].Add(AttackResult.Kill, " +
                                      resultAverages[a][d][AttackResult.Kill] + ");");
                    System.Diagnostics.Debug.WriteLine("resultAverages[" + a + "][" + d + "].Add(AttackResult.Push, " +
                                      resultAverages[a][d][AttackResult.Push] + ");");
                    System.Diagnostics.Debug.WriteLine("resultAverages[" + a + "][" + d + "].Add(AttackResult.CounterPush, " +
                                      resultAverages[a][d][AttackResult.CounterPush] + ");");
                    System.Diagnostics.Debug.WriteLine("resultAverages[" + a + "][" + d + "].Add(AttackResult.CounterKill, " +
                                      resultAverages[a][d][AttackResult.CounterKill] + ");");
                }
            }
        }
    }
}


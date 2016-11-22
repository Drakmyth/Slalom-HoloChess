using Random = System.Random;

namespace DejarikLibrary
{
	public enum AttackResult
	{
		Kill,
		CounterKill,
		Push,
		CounterPush
	}

	public class AttackCalculator
	{
		private readonly Random _random;

		public AttackCalculator()
		{
			_random = new Random();
		}

		public AttackResult Calculate(int attack, int defense)
		{
			const int sidesOnDie = 6;

			int sum = 0;
			for (int i = 0; i < attack; i++)
			{
				sum += _random.Next(sidesOnDie) + 1;
			}

			int totalAttack = sum;

			sum = 0;
			for (int i = 0; i < defense; i++)
			{
				sum += _random.Next(sidesOnDie) + 1;
			}

			int totalDefense = sum;

			if (totalAttack - totalDefense >= 7)
			{
				return AttackResult.Kill;
			}
			if (totalAttack - totalDefense < 7 && totalAttack - totalDefense > 0)
			{
				return AttackResult.Push;
			}
			if (totalDefense - totalAttack >= 7)
			{
				return AttackResult.CounterKill;
			}

			return AttackResult.CounterPush;
		}
	}
}
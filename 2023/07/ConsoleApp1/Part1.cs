namespace Part1
{
    public class Part1
    {
        public static int Run(IEnumerable<string[]> hands)
        {
            var orderedHands = hands.Order(new HandComparer());
            return orderedHands.Select((hand, index) => (hand, index)).Sum(x => int.Parse(x.hand.Last()) * (x.index + 1));
        }
    }

    class HandComparer : IComparer<string[]>
    {
        public int Compare(string[] hand1, string[] hand2)
        {
            var handOrder = new[] { Is5OfAKind, Is4OfAKind, IsFullHouse, IsThreeOfAKind, IsTwoPairs, IsOnePair };
            var cardOrder = new[] { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };

            foreach (var check in handOrder)
            {
                var firstHandMatches = check(hand1.First());
                var secondHandMatches = check(hand2.First());
                if (firstHandMatches != secondHandMatches)
                    return firstHandMatches ? 1 : -1;
				if (firstHandMatches && secondHandMatches)
					break;
			}

			for (int i = 0; i < hand1.First().Length; i++)
            {
                var firstCard = Array.IndexOf(cardOrder, hand1.First()[i]);
                var secondCard = Array.IndexOf(cardOrder, hand2.First()[i]);
                if (firstCard < secondCard)
                    return 1;
                if (firstCard > secondCard)
                    return -1;
            }
            return 0;

            bool Is5OfAKind(string hand) => hand.GroupBy(card => card).Any(group => group.Count() == 5);
            bool Is4OfAKind(string hand) => hand.GroupBy(card => card).Any(group => group.Count() == 4);
            bool IsFullHouse(string hand) => hand.GroupBy(card => card).Any(group => group.Count() == 3) && hand.GroupBy(card => card).Any(group => group.Count() == 2);
            bool IsThreeOfAKind(string hand) => hand.GroupBy(card => card).Any(group => group.Count() == 3);
            bool IsTwoPairs(string hand) => hand.GroupBy(card => card).Count(group => group.Count() == 2) == 2;
            bool IsOnePair(string hand) => hand.GroupBy(card => card).Any(group => group.Count() == 2);
        }
    }
}
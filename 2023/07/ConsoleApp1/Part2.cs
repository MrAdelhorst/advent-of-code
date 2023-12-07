namespace Part2
{
    public class Part2
    {
        public static int Run(IEnumerable<string[]> hands)
        {
            var orderedHands = hands.Order(new HandComparer2());
            return orderedHands.Select((hand, index) => (hand, index)).Sum(x => int.Parse(x.hand.Last()) * (x.index + 1));
        }
    }

    class HandComparer2 : IComparer<string[]>
    {
        public int Compare(string[] hand1, string[] hand2)
        {
            var firstHand = GetHandCategory(hand1.First());
            var secondHand = GetHandCategory(hand2.First());
            if (firstHand > secondHand)
                return 1;
            if (firstHand < secondHand)
                return -1;

            var cardOrder = new[] { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J' };
            for (int i = 0; i < hand1.First().Length; i++)
            {
                var firstCard = Array.IndexOf(cardOrder, hand1.First()[i]);
                var secondCard = Array.IndexOf(cardOrder, hand2.First()[i]);
                if (firstCard < secondCard)
                    return 1;
                else if (firstCard > secondCard)
                    return -1;
            }
            return 0;
        }

        enum HandCategory { HighCard, OnePair, TwoPairs, ThreeOfAKind, FullHouse, FourOfAKind, FiveOfAKind }
        private HandCategory GetHandCategory(string hand)
        {
            if (hand == "JJJJJ")
                return HandCategory.FiveOfAKind;

            var groups = hand.Replace("J", "").GroupBy(card => card);
            var jokers = hand.Count(h => h == 'J');
            var orderedGroups = groups.OrderByDescending(group => group.Count());
            switch (orderedGroups.First().Count() + jokers)
            {
                case 5:
                    return HandCategory.FiveOfAKind;
                case 4:
                    return HandCategory.FourOfAKind;
                case 3:
                    return orderedGroups.Skip(1).First().Count() == 2 ? HandCategory.FullHouse : HandCategory.ThreeOfAKind;
                case 2:
                    return orderedGroups.Skip(1).First().Count() == 2 ? HandCategory.TwoPairs : HandCategory.OnePair;
                case 1:
                    return HandCategory.HighCard;
                default:
                    throw new Exception("Invalid hand");
            }
        }
    }
}
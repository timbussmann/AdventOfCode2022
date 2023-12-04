namespace AdventOfCode22;

public class Day13
{
    [Fact]
    public async Task Part1()
    {
        var input = await File.ReadAllLinesAsync("day13.txt");

        var orderedPacketIndexes = FindOrderedPackets(input);
        var result = orderedPacketIndexes.Sum();
        
        Assert.Equal(13, result);
    }

    private IEnumerable<int> FindOrderedPackets(string[] input)
    {
        List<(IPacketItem left, IPacketItem right)> packetGroups = new ();
        for (int i = 0; i < input.Length; i+=3)
        {
            packetGroups.Add(new (Parse(input[i]), Parse(input[i+1])));
        }

        int total = 0;
        for (int i = 0; i < packetGroups.Count; i++)
        {
            if (Compare(packetGroups[i].left, packetGroups[i].right) < 0)
            {
                yield return i + 1;
            }
        }
    }
    
    static int Compare(IPacketItem left, IPacketItem right)
    {
        if (left is Digit dl)
        {
            if (right is Digit dr)
            {
                return dl.digit.CompareTo(dr.digit);
            }

            if (right is PacketList rl)
            {
                return Compare(new PacketList() { dl }, rl);
            }
        } else if (left is PacketList ll)
        {
            if (right is Digit dr)
            {
                return Compare(ll, new PacketList() { dr });
            }

            if (right is PacketList rl)
            {
                for (int i = 0; i < ll.Count; i++)
                {
                    if (i >= rl.Count)
                    {
                        // right list ran out of items before left one, left is bigger
                        return 1;
                    }

                    var comparison = Compare(ll[i], rl[i]);
                    if (comparison == 0)
                    {
                        continue;
                    }

                    return comparison;
                }

                // left list ran out of items before right list
                return -1;
            }
        }

        throw new Exception("invalid");
    }

    static IPacketItem Parse(string line)
    {
        PacketList currentItems = new();
        for (int i = 1; i < line.Length - 1; i++)
        {
            var c = line[i];
            if (char.IsDigit(c))
            {
                currentItems.Add(new Digit(int.Parse(c + "")));
            } else if (c == ',')
            {
                // continue
            } else if (c == '[')
            {
                var subList = new PacketList();
                currentItems.Add(subList);
                subList.Parent = currentItems;
                currentItems = subList;
            }
            else if (c == ']')
            {
                currentItems = currentItems.Parent;
            }
        }

        return currentItems;
    }

    interface IPacketItem
    {
    }

    record Digit(int digit) : IPacketItem
    {
    }

    class PacketList : List<IPacketItem>, IPacketItem
    {
        public PacketList Parent { get; set; }
    }
}
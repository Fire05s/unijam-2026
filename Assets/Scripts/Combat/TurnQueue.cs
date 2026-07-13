using System.Collections.Generic;
using System.Linq;

public class TurnData {
    public int dinoID = -1;
    public bool canAttackOnce = true;
}

public class TurnQueue {
    private SortedDictionary<int, TurnData> _queue;

    public TurnQueue() {
        _queue = new();
    }

    public int Enqueue(int desiredTurn, int id, List<WildCard> wildcards) {
        int finalTurn = desiredTurn;
        while (_queue.TryGetValue(desiredTurn, out TurnData value)) {
            finalTurn++;
        }

        TurnData newTurn = new TurnData
        {
            dinoID = id
        };
        foreach (WildCard wc in wildcards) {
            switch (wc) {
                case WildCard.Doublehit: newTurn.canAttackOnce = false; break;
                // other cases
                default: continue;
            }
        }
        _queue[finalTurn] = newTurn;

        return finalTurn;
    }

    public KeyValuePair<int, TurnData> Dequeue() {
        KeyValuePair<int, TurnData> dequeuedTurn = _queue.First();
        _queue.Remove(dequeuedTurn.Key);
        return dequeuedTurn;
    }

    public List<KeyValuePair<int, TurnData>> PeekX(int count) {
        List<KeyValuePair<int, TurnData>> nextTurns = new();
        int index = 0;
        foreach (KeyValuePair<int, TurnData> kvp in _queue) {
            if (index >= count) {
                break;
            }
            nextTurns.Add(kvp);
        }
        return nextTurns;
    }
}
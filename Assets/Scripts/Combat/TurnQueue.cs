using System.Collections.Generic;
using System.Linq;

namespace Combat
{
    /// <summary>
    /// A data object that holds an attacking dino's ID and booleans on
    /// </summary>
    public class TurnData
    {
        public TurnData(int id, bool atq=true) {dinoID = id; addToQueue = atq;}
        public int dinoID = -1;
        public bool addToQueue = true;
        public bool canMultihit = false; // Multihit
        public bool causesBleed = false; // Bleed
        public bool canAttackAgain = false; // Doublehit, Bloodlust
        public bool canSelfHeal = false; // Ravenous Bite, Scavenger
        public bool canTeamHeal = false; // Pack Treats
        public bool canApplyMark = false; // Pack Mentality

        // Combat Manager will need a flag for successful kills (for Bloodlust and Scavenger)
        // Combat Manager will need to remember if a a crit has successfully happened in a turn
        //      If so, apply the same attack again until there is no crit (Lucky Streak)
        // Combat Manager will need to handle Dodge for defenders
    }

    public class TurnQueue
    {
        private SortedDictionary<int, TurnData> _queue;

        public TurnQueue()
        {
            _queue = new();
        }

        /// <summary>
        /// Ask to queue a specific turn. If that slot is already filled, give the next unfilled slot.
        /// Creates a turn in the queue with the final turn number and any turn relevant data (ID and wildcards).
        /// </summary>
        /// <param name="desiredTurn"> The turn that this dino would like to act next </param>
        /// <param name="id"> The ID of the dino requesting an attack </param>
        /// <param name="wildcards"> The wildcard abilities this requesting dino has </param>
        /// <returns> The turn that the dino will be acting next </returns>
        public int Enqueue(int desiredTurn, int id, List<WildCard> wildcards)
        {
            int finalTurn = desiredTurn;

            while (_queue.TryGetValue(finalTurn, out TurnData value))
            {
                finalTurn++;
            }

            TurnData newTurn = new TurnData(id);

            foreach (WildCard wc in wildcards)
            {
                switch (wc) {
                    case WildCard.Multihit: newTurn.canMultihit = true; break;
                    case WildCard.Bleed: newTurn.causesBleed = true; break;
                    case WildCard.Doublehit: newTurn.canAttackAgain = true; break;
                    case WildCard.Bloodlust: newTurn.canAttackAgain = true; break;
                    case WildCard.Ravenousbite: newTurn.canSelfHeal = true; break;
                    case WildCard.Scavenger: newTurn.canSelfHeal = true; break;
                    case WildCard.Packtreats: newTurn.canTeamHeal = true; break;
                    case WildCard.Packmentality: newTurn.canApplyMark = true; break;
                    default: break;
                }
            }
            _queue[finalTurn] = newTurn;

            return finalTurn;
        }
        // public int PriorityEnqueue(int turn, int id)
        // {
        //     int slot = turn;
        //     while(_queue.TryGetValue())
        // }

        /// <summary>
        /// Dequeue the next turn, removing it from the turn queue and returning it.
        /// </summary>
        /// <returns> The key-value pair of the turn number (int) and a TurnData object </returns>
        public (int, TurnData) Dequeue()
        {
            KeyValuePair<int, TurnData> dequeuedTurn = _queue.First();
            _queue.Remove(dequeuedTurn.Key);
            return (dequeuedTurn.Key, dequeuedTurn.Value);
        }

        /// <summary>
        /// Look X turns ahead, receiving a list of at most X dinosaurs that will be acting next.
        /// If there are not enough dinosaurs in the queue, nothing more will be returned.
        /// Preferred to be used on turn complete instead of every frame.
        /// </summary>
        /// <param name="count">
        ///     How many dinosaurs should be looked for (target worst case where all slots in a 
        ///     UI representation are filled with dinos) 
        /// </param>
        /// <returns> A list of key-value pairs of turn number (int) and a TurnData object </returns>
        public List<KeyValuePair<int, TurnData>> PeekX(int count)
        {
            List<KeyValuePair<int, TurnData>> nextTurns = new();
            int index = 0;
            foreach (KeyValuePair<int, TurnData> kvp in _queue)
            {
                if (index >= count) {
                    break;
                }
                nextTurns.Add(kvp);
            }
            return nextTurns;
        }
    }
}
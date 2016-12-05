namespace Assets.Scripts.AI
{
    class GonkDroidAI : Client
    {

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            

        }


        private void AwaitSubActionSixSelection()
        {
            /*
            if (ActionNumber == 3 || ActionNumber == 4)
            {               
                //TODO: bake this into gonk droid et al
                Node aiActionNode;
                if (AvailablePushDestinations.Any())
                {
                    aiActionNode = AvailablePushDestinations.ElementAt(_random.Next(AvailablePushDestinations.Count()));
                }
                else
                {
                    SubActionNumber = 0;
                    return;
                }

                OnSpaceSelected(aiActionNode.Id);

            }
            */
        }


        private void AwaitSubActionSevenSelection()
        {
            //TODO: Net awaitResult
            /*
            if (ActionNumber == 1 || ActionNumber == 2)
            {

                //TODO: bake this into gonk droid et al
                Node aiActionNode;
                if (AvailablePushDestinations.Any())
                {
                    aiActionNode = AvailablePushDestinations.ElementAt(_random.Next(AvailablePushDestinations.Count()));
                }
                else
                {
                    SubActionNumber = 0;
                    return;
                }

                OnSpaceSelected(aiActionNode.Id);

            }
            */
        }


        /*
        private void AwaitSubActionTwoSelection()
        {
            if (ActionNumber == 3 || ActionNumber == 4)
            {
                IEnumerable<BoardSpace> availableSpaces =
                    BoardSpaces.Values.Where(s => Player2Monsters.Select(m => m.CurrentNode.Id).Contains(s.Node.Id)).ToList();
                //TODO: bake this into gonk droid et al
                if (availableSpaces.Any())
                {
                    BoardSpace aiChoice = availableSpaces.ElementAt(_random.Next(availableSpaces.Count()));

                    OnSpaceSelected(aiChoice.Node.Id);

                }
            }

        }
        */

        private void AwaitSubActionFourSelection()
        {
            /*
            if (ActionNumber == 3 || ActionNumber == 4)
            {
                IEnumerable<Node> friendlyOccupiedNodes = Player2Monsters.Select(monster => monster.CurrentNode).ToList();
                IEnumerable<Node> enemyOccupiedNodes = Player1Monsters.Select(monster => monster.CurrentNode).ToList();

                IEnumerable<int> availableMoveActionNodeIds = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                    SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(a => a.DestinationNode.Id);

                IEnumerable<int> availableAttackActionNodeIds = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                    enemyOccupiedNodes).Select(a => a.Id);
                //TODO: bake this into gonk droid et al
                int aiActionNodeId;
                if (availableAttackActionNodeIds.Any())
                {
                    aiActionNodeId = availableAttackActionNodeIds.ElementAt(_random.Next(availableAttackActionNodeIds.Count()));
                }
                else if (availableMoveActionNodeIds.Any())
                {
                    aiActionNodeId = availableMoveActionNodeIds.ElementAt(_random.Next(availableMoveActionNodeIds.Count()));
                }
                else
                {
                    SubActionNumber = 2;
                    return;
                }

                OnSpaceSelected(aiActionNodeId);

            }
            */
        }
    }
}
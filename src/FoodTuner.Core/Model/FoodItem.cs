namespace FoodTuner.Model
{
    public class FoodItem
    {
        public FoodItem(string name, string gameId)
        {
            Name = name; 
            GameId = gameId;
        }

        public string Name { get; set; }

        public string GameId { get; set; }

        public Biomes Biome { get; set; }

        public FoodComplexity Complexity { get; set; }

        public bool BudgetBonus { get; set; }

        public int Health { get; set; }

        public int Stamina { get; set; }

        public int Regen { get; set; }

        public int Duration { get; set; }

        public int Endurance { get; set; }

        public double Weight { get; set; }

        public int Stack { get; set; }
    }
}

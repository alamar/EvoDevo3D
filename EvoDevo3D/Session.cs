using System;


namespace EvoDevo4
{
    public class Session
    {
        public GeneticCode Controls { get; }
        public Simulation Simulation { get; }
        public EvoArea EvoArea { get; }

        public Session(GeneticCode code, Simulation simulation, EvoArea evoArea) {
            this.Controls = code;
            this.Simulation = simulation;
            this.EvoArea = evoArea;
        }

        public void resume() {
            Simulation.paused = false;
        }

        public bool toggle() {
            Simulation.paused = !Simulation.paused;
            return !Simulation.paused;
        }
    }
}

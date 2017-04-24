using System;

namespace EvoDevo4
{
    public class Session
    {
        private GeneticCode controls;
        public GeneticCode Controls
        {
            get
            {
                return controls;
            }
        }
        

        private Simulation simulation;
        public Simulation Simulation
        {
            get
            {
                return simulation;
            }
        }

        private EvoArea display;

        public Session(GeneticCode controls, Simulation simulation, EvoArea display)
        {
            this.controls = controls;
            this.simulation = simulation;
            this.display = display;
        }

        public void resume()
        {
            this.simulation.paused = false;
        }

        public Boolean toggle()
        {
            this.simulation.paused = !this.simulation.paused;
            return !this.simulation.paused;
        }
    }
}


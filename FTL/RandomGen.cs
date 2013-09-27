using System;
using System.Collections.Generic;
using System.Text;

namespace FTL {
    public class RandomGen {
        Random r = new Random();

        public int MaxStep { get; set; }
        public int MinStep { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public int MaxDeltaDown { get; set; }
        public int MaxDeltaUp { get; set; }
        public int StartHeight { get; set; }

        public int Length { get; set; }

        int getRandHeight(int current) {
            int ht = r.Next(Math.Max(MinHeight, current - MaxDeltaDown),Math.Min(MaxHeight, current + MaxDeltaUp));
            return ht;
        }

        int getStep() {
            return r.Next(MinStep,MaxStep);
        }

        public int[] Generate() {
            int ht = StartHeight;
            List<int> heights = new List<int>();

            int runLen = Length;
            while (runLen > 0) {
                int currentStep = getStep();
                if (runLen - currentStep <= 0)
                    currentStep = runLen;
                for (int i = 0; i < currentStep; ++i)
                    heights.Add(ht);

                runLen -= currentStep;
                ht = getRandHeight(ht);
            }

            return heights.ToArray();
        }

        public int[,] GetTiles() {
            int[] cols = Generate();
            int[,] cells = new int[cols.Length,MaxHeight];
            for (int x = 0; x < cols.Length; ++x) {
                for (int i = 0; i < MaxHeight; ++i) //open cell
                    cells[x,i] = 0;
                for (int y = 0; y < cols[x]; ++y) { //closed cell
                    cells[x,y] = 1;
                }
            }
            return cells;
        }

        public string getString() {
            int[] vals = Generate();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vals.Length; ++i) {
                if (i > 0)
                    sb.Append(",");
                sb.Append(vals[i]);
            }
            return sb.ToString();
        }

        /*
         * W Type, not agile, melee/bows
         * R Type, agile PoP, poor melee/bows
         * M Type, not agile, glass cannon
        */
    }
}

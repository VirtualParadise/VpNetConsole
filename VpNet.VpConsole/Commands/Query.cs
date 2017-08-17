using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VpNet.CommandLine;
using VpNet.CommandLine.Attributes;
using VpNet.Extensions;
using VpNet.VpConsoleServices.PluginFramework;
using VpNet.VpConsoleServices.PluginFramework.Interfaces;

namespace VpNet.VpConsole.Commands
{
    [Command(Literal = "query")]
    public class Query : IParsableCommand<VpPluginContext>
    {
        [Literal(Required = true, ArgumentIndex = 1)]
        public int Cell1X { get; set; }
        [Literal(Required = true, ArgumentIndex = 2)]
        public int Cell1Z { get; set; }
        [Literal(Required = true, ArgumentIndex = 3)]
        public int Cell2X { get; set; }
        [Literal(Required = true, ArgumentIndex = 4)]
        public int Cell2Z { get; set; }
        [Literal(Required = false, ArgumentIndex = 5)]
        public string OutputFile { get; set; }

        private static IConsole _cli;
        private Stopwatch _st;


        public bool Execute(VpPluginContext executionContext)
        {
            _sender = executionContext.Vp;
            if (stat == null)
            {
                _st = new Stopwatch();
                _cli = executionContext.Cli;
                executionContext.Vp.OnQueryCellRangeEnd += Vp_OnQueryCellRangeEnd;
                _st.Start();
                // executionContext.Vp.AddCellRange(new Cell(-40, -40), new Cell(40, 40));
                executionContext.Vp.AddCellRange(new Cell(0, 6), new Cell(0, 6));

                //if (Cell1X==0&&Cell2X==0&&Cell1Z==0&&Cell2Z==0)
                //    executionContext.Cli.WriteLine(ConsoleMessageType.Error,"?Query please enter valid arguments to performa query.");
                executionContext.Cli.WriteLine(ConsoleMessageType.Normal, "Executing cell query...");

                // executionContext.Vp.AddCellRange(new Cell(Cell1X, Cell1Z), new Cell(Cell2X, Cell2Z));
            }
            else
            {
                printStats(_sender.CacheObjects.Copy());
            }
            return true;

        }

        private static Dictionary<string, Stats> stat; 

        private class Stats
        {
            public int Points { get; set; }
            public int MoveablePoints { get; set; }
            public int Cubes { get; set; }
        }

        void printStats(IEnumerable<VpObject<Vector3>> cache)
        {
            lock (_sender)
            {
                stat = new Dictionary<string, Stats>();
                foreach (var j in cache)
                {
                    if (j.Owner != 5)
                        continue;
                    var l = j.Description.Replace("\r", "").Split(new[] {'\n'});
                    if (l.Length == 2)
                    {
                        if (!stat.ContainsKey(l[0]))
                        {
                            stat[l[0]] = new Stats();
                        }
                        var p = int.Parse(l[1].TrimStart('(').TrimEnd(')'));
                        stat[l[0]].Cubes++;
                        stat[l[0]].Points += p;
                        if (p > 1)
                            stat[l[0]].MoveablePoints += p - 1;

                    }

                }
            }
            string s = "";
            _cli.WriteLine(string.Format("Cubes\tPoints\tSelectable\tPLayer"));
            s += string.Format("Cubes\tPoints\tSelectable\tPLayer") + "\r\n";
            foreach (var p in stat)
            {
                _cli.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", p.Value.Cubes, p.Value.Points,p.Value.MoveablePoints, p.Key));
                s += string.Format("{0}\t{1}\t{2}\t{3}", p.Value.Cubes, p.Value.Points, p.Value.MoveablePoints, p.Key) + "\r\n";
            }

            var m = _sender.CacheObjects.Find(p => p.Model == "zsign");
            {
                m.Description = s;
            }
       //     _sender.ChangeObject(m);
        }

        private Instance _sender;

        void Vp_OnQueryCellRangeEnd(Instance sender, CellRangeQueryCompletedArgs<VpObject<Vector3>, Vector3> args)
        {
            _st.Stop();
            sender.OnQueryCellRangeEnd -= Vp_OnQueryCellRangeEnd;
            sender.OnObjectCellRangeChange += sender_OnObjectCellRangeChange;
            sender.OnObjectCellRangeCreate += sender_OnObjectCellRangeCreate;
            sender.OnObjectCellRangeDelete += sender_OnObjectCellRangeDelete;
            _cli.WriteLine(ConsoleMessageType.Normal, string.Format("Cell query finished in {0} found {1} objects.",_st.Elapsed,args.VpObjects.Count));
            var o = args.VpObjects.Where(p => p.Model == "gamecube");
            _cli.WriteLine(ConsoleMessageType.Event, "cubes: " + o.Count());
            printStats(_sender.CacheObjects.Copy());
            _st.Reset();
        }

        void sender_OnObjectCellRangeDelete(Instance sender, ObjectChangeArgsT<Avatar<Vector3>, VpObject<Vector3>, Vector3> args)
        {
            //if (args.VpObject.Model == "gamecube")
            //{
            //    printStats(sender.CacheObjects.Copy());
            //}
        }

        void sender_OnObjectCellRangeCreate(Instance sender, ObjectChangeArgsT<Avatar<Vector3>, VpObject<Vector3>, Vector3> args)
        {
            //if (args.VpObject.Model == "gamecube")
            //{
            //    printStats(sender.CacheObjects.Copy());
            //}
        }

        void sender_OnObjectCellRangeChange(Instance sender, ObjectChangeArgsT<Avatar<Vector3>, VpObject<Vector3>, Vector3> args)
        {
            //if (args.VpObject.Model == "gamecube")
            //{
               _cli.WriteLine(ConsoleMessageType.Event,args.VpObject.Serialize());
            //    printStats(sender.CacheObjects.Copy());
            //}
        }
    }
}

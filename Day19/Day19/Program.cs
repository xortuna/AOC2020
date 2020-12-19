using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day19
{
    class Program
    {
        class RuleCacheItem
        {
            public string Text;
            public IRule CachedRule = null;
        }

        interface IRule
        {
            IEnumerable<int> Match(int from, string msg, int recursion);
        }

        abstract class Function : IRule
        {
            public List<IRule> childRules = new List<IRule>();
            public void AddNode(IRule rule)
            {
                childRules.Add(rule);
            }

            public abstract IEnumerable<int> Match(int from, string msg, int recursion);

            public void ReplaceNode(IRule node, IRule newNode)
            {
                for (int i = 0; i < childRules.Count; ++i)
                {
                    if (childRules[i] == node)
                        childRules[i] = newNode;
                }
            }
        }

        class StringRule : IRule
        {
            public char toMatch;
            public IEnumerable<int> Match(int from, string msg, int recursion)
            {
                if (msg.Length > from && (msg[from].Equals(toMatch)))
                {
                    return new List<int> { from + 1 };
                }
                return Enumerable.Empty<int>(); //OOB
            }
        }

        class AndRule : Function
        {
            public override IEnumerable<int> Match(int from, string msg, int recursion)
            {
                IEnumerable<int> toTest = new List<int> { from };
                foreach(var rule in childRules)
                {
                    bool matching = false;
                    foreach (var t in toTest)
                    {
                        var res = rule.Match(t, msg, recursion);
                        if (res.Any())
                        {
                            matching = true;
                            toTest = res;
                            break;
                        }
                    }
                    if (!matching)
                        return Enumerable.Empty<int>();
                }
                return toTest;
            }
            
        }

        class OrRule : Function
        {
            public override IEnumerable<int> Match(int from, string msg, int recursion)
            {
                if (recursion < 0)
                    return Enumerable.Empty<int>();

                IEnumerable<int> combinationMatches = new List<int>();

                // we actually have to go down both branches because the position in string the may be different per match, which may affect our parent rules
                combinationMatches = combinationMatches.Concat(childRules[0].Match(from, msg, recursion-1));
                combinationMatches = combinationMatches.Concat(childRules[1].Match(from, msg, recursion-1));
                
                return combinationMatches;
            }
        }

        static void Main(string[] args)
        {
            Dictionary<int, RuleCacheItem> ruleParse = new Dictionary<int, RuleCacheItem>();
            List<string> toCheck = new List<string>();
            foreach(var line in  System.IO.File.ReadLines("puzzleinput.txt"))
            {
                if (line.Contains(':'))
                {
                    var c = line.Split(':');
                    ruleParse.Add(int.Parse(c[0]), new RuleCacheItem { Text = c[1].Substring(1) });
                }
                else if(!string.IsNullOrEmpty(line))
                {
                    toCheck.Add(line);
                }
            }


            //build tree
            var currentNode = ruleParse[0];
            currentNode.CachedRule = CompileNodeForRule(currentNode.Text, ruleParse);

            int SuccessCount = toCheck.Count(line => currentNode.CachedRule.Match(0, line, 100).Any(t=>t == line.Length));
            Console.WriteLine($"Part 1: {SuccessCount}");

            //Disgusting hack to replace nodes 8 and 11:

            var oldEight = ruleParse[8].CachedRule;

            //Update rule 8
            ruleParse[8].CachedRule = new OrRule();
            //LHS
            var lhs = new AndRule();
                lhs.AddNode(ruleParse[42].CachedRule);
            //RHS
            var rhs = new AndRule();
                rhs.AddNode(ruleParse[42].CachedRule);
                rhs.AddNode(ruleParse[8].CachedRule);

            ((Function)ruleParse[8].CachedRule).AddNode(lhs);
            ((Function)ruleParse[8].CachedRule).AddNode(rhs);

            //Update rule 11
            var oldEleven = ruleParse[11].CachedRule;
            ruleParse[11].CachedRule = new OrRule();
            //LHS
            lhs = new AndRule();
                lhs.AddNode(ruleParse[42].CachedRule);
                lhs.AddNode(ruleParse[31].CachedRule);
            //RHS
            rhs = new AndRule();
                rhs.AddNode(ruleParse[42].CachedRule);
                rhs.AddNode(ruleParse[11].CachedRule);
                rhs.AddNode(ruleParse[31].CachedRule);

            ((Function)ruleParse[11].CachedRule).AddNode(lhs);
            ((Function)ruleParse[11].CachedRule).AddNode(rhs);

            //Replace the old 8's and 11's with the new ones
            foreach(var node in ruleParse.Values)
            {
                if(typeof(Function).IsAssignableFrom(node.CachedRule.GetType()))
                {
                    ((Function)node.CachedRule).ReplaceNode(oldEight, ruleParse[8].CachedRule);
                    ((Function)node.CachedRule).ReplaceNode(oldEleven, ruleParse[11].CachedRule);
                }
            }

            SuccessCount = toCheck.Count(line => currentNode.CachedRule.Match(0, line, 100).Any(t => t == line.Length));
            Console.WriteLine($"Part 2: {SuccessCount}");
            Console.ReadLine();

        }

        static IRule CompileNodeForRule(string ruletext, Dictionary<int, RuleCacheItem> ruleSet)
        {
            Function Head, BuildingRule;
            Head = BuildingRule = new AndRule();

            foreach (var item in ruletext.Split(' '))
            {
                if (item.Contains("\""))
                {
                    var nodeToAdd = new StringRule { toMatch = item[1] }; ;

                    //Comment this line for multi string rule support
                    return nodeToAdd;

                    BuildingRule.AddNode(nodeToAdd);
                }
                else if (item.Contains("|"))
                {
                    //Split on left and right
                    Head = new OrRule();
                    Head.AddNode(BuildingRule);
                    BuildingRule = new AndRule();
                    Head.AddNode(BuildingRule);
                }
                else
                {
                    var childToExplore = int.Parse(item);
                    //Check if the cached copy hasn't compiled yet and compile it
                    if (ruleSet[childToExplore].CachedRule == null)
                        ruleSet[childToExplore].CachedRule = CompileNodeForRule(ruleSet[childToExplore].Text, ruleSet);

                    BuildingRule.AddNode(ruleSet[childToExplore].CachedRule);
                }
            }
            return Head;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day19
{
    class Program
    {
        class RuleDefiniton
        {
            public string Text;
            public IRule CachedRule = null;
        }

        interface IRule
        {
            bool Match(ref int from, string msg);
        }
        interface IFunction : IRule
        {
            void AddNode(IRule rule);
            void ReplaceNode(IRule node, IRule newNode);
        }


        class StringRule : IRule
        {
            public char toMatch;
            public bool Match(ref int from, string msg)
            {
                if (msg.Length <= from)
                    return false; //OOB
                return msg[from++].Equals(toMatch);
            }
        }
        class AndRule : IFunction
        {
            public List<IRule> childRules = new List<IRule>();

            public void AddNode(IRule rule)
            {
                childRules.Add(rule);
            }

            public bool Match(ref int from, string msg)
            {
                foreach(var rule in childRules)
                {
                    if (!rule.Match(ref from, msg))
                        return false;
                }
                return true;
            }
            public void ReplaceNode(IRule node, IRule newNode)
            {
                for (int i = 0; i < childRules.Count; ++i)
                {
                    if (childRules[i] == node)
                        childRules[i] = newNode;
                }
            }
        }

        class OrRule : IFunction
        {
            public List<IRule> childRules = new List<IRule>();

            public void AddNode(IRule rule)
            {
                childRules.Add(rule);
            }
            public bool Match(ref int from, string msg)
            {
                int temp = from;
                if(childRules[0].Match(ref temp, msg))
                {
                    from = temp;
                    return true;
                }
                else if(childRules[1].Match(ref from, msg))
                    return true;

                return false;
            }

            public void ReplaceNode(IRule node, IRule newNode)
            {
                for(int i=0; i < childRules.Count; ++i)
                {
                    if (childRules[i] == node)
                        childRules[i] = newNode;
                }
            }
        }

        static void Main(string[] args)
        {
            Dictionary<int, RuleDefiniton> ruleParse = new Dictionary<int, RuleDefiniton>();
            List<string> toCheck = new List<string>();
            foreach(var line in  System.IO.File.ReadLines("puzzleinput.txt"))
            {
                if (line.Contains(':'))
                {
                    var c = line.Split(':');
                    ruleParse.Add(int.Parse(c[0]), new RuleDefiniton { Text = c[1].Substring(1) });
                }
                else if(!string.IsNullOrEmpty(line))
                {
                    toCheck.Add(line);
                }
            }


            //build tree
            var currentNode = ruleParse[0];
            currentNode.CachedRule = CompileNodeForRule(currentNode.Text, ruleParse);

            int SuccessCount = 0;
            foreach(var line in toCheck)
            {
                int ptr = 0;
                if(currentNode.CachedRule.Match(ref ptr, line) && ptr == line.Length)
                    SuccessCount++;
            }

            Console.WriteLine($"Part 1: {SuccessCount}");


            var toDelete1 = ruleParse[8].CachedRule;
            //Update rule 8
            ruleParse[8].CachedRule = new OrRule();
            //LHS
            var lhs = new AndRule();
            ((IFunction)lhs).AddNode(ruleParse[42].CachedRule);
            //RHS
            var rhs = new AndRule();
                rhs.AddNode(ruleParse[42].CachedRule);
                rhs.AddNode(ruleParse[8].CachedRule);

            ((IFunction)ruleParse[8].CachedRule).AddNode(lhs);
            ((IFunction)ruleParse[8].CachedRule).AddNode(rhs);

            //Update rule 11

            var toDelete2 = ruleParse[11].CachedRule;
            ruleParse[11].CachedRule = new OrRule();
            //LHS
            lhs = new AndRule();
            ((IFunction)lhs).AddNode(ruleParse[42].CachedRule);
            ((IFunction)lhs).AddNode(ruleParse[31].CachedRule);
            //RHS
            rhs = new AndRule();
                rhs.AddNode(ruleParse[42].CachedRule);
                rhs.AddNode(ruleParse[11].CachedRule);
                rhs.AddNode(ruleParse[31].CachedRule);

            ((IFunction)ruleParse[11].CachedRule).AddNode(lhs);
            ((IFunction)ruleParse[11].CachedRule).AddNode(rhs);

            foreach(var node in ruleParse.Values)
            {
                if(typeof(IFunction).IsAssignableFrom(node.CachedRule.GetType()))
                {
                    ((IFunction)node.CachedRule).ReplaceNode(toDelete1, ((IFunction)ruleParse[8].CachedRule));
                    ((IFunction)node.CachedRule).ReplaceNode(toDelete2, ((IFunction)ruleParse[11].CachedRule));
                }
            }

            SuccessCount = 0;
            foreach (var line in toCheck)
            {
                int ptr = 0;
                if (currentNode.CachedRule.Match(ref ptr, line) && ptr == line.Length)
                    SuccessCount++;

            }
            Console.WriteLine($"Part 2: {SuccessCount}");
            Console.ReadLine();

        }

        static IRule CompileNodeForRule(string ruletext, Dictionary<int, RuleDefiniton> ruleSet)
        {
            IFunction Head, BuildingRule;
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompiledScript.Compiler
{
    class SourceCodeReader
    {
        public Node Read(string source)
        {
            Node root = new Node(false);
            root.Text = "root";
            Read(source, root, 0);
            return root;
        }

        private int Read(string source, Node root, int position)
        {
		    int i = position;
		    int taille = source.Length;
            Node current = root;
            Node parent;
		    Node tmp;

		    char car;

		    while (i < taille)
            {
			    i = Skip(source, i, new char[]{' ', '\t', '\n', ';', '\r'} );

			    if (i >= taille)
                {
				    break;
			    }

			    car = source.ElementAt(i);

			    if (car == '(')
                {
				    parent = current;
				    current = new Node(false);
				    current.Text = "()";

				    current.Parent = parent;
				    parent.Children.AddLast(current);
			    }
                else if (car == ')')
                {
				    if (current.Parent != null)
                    {
					    current = current.Parent;
				    }
			    }
                else if (car == ',')
                {
				    // Ignore.
			    }
                else if (car == '"' || car == '\'')
                {
                    i++;
				    string words = ReadString(source, ref i, car);
				    tmp = new Node(true);
				    tmp.Text = words;
				    current.Children.AddLast(tmp);
			    }
                else
                {
                    string word = ReadWord(source, i);
				    if (word.Length != 0)
                    {
					    i += word.Length;

                        i = Skip(source, i, new char[] { ' ', '\t', '\n', '\r' });
					    if (i >= taille)
                        {
						    // TODO ... handle ?
						    break;
					    }
					    car = source.ElementAt(i);
					    if (car == '(')
                        {
						    parent = current;
						    current = new Node(false);
						    current.Text = word;

						    current.Parent = parent;
						    parent.Children.AddLast(current);
					    }
                        else
                        {
                            //if (car == ')')
                            //{
							    i--;
						    //}
						    tmp = new Node(true);
						    tmp.Text = word;
						    current.Children.AddLast(tmp);
					    }
				    }
			    }
			    i++;
		    }
		    return i;
	    }

        public static int Skip(string exp, int position, char[] cars)
        {
            int i = position;
            int taille = exp.Length;
            char car;
            bool found;

            while (i < taille)
            {
                car = exp.ElementAt(i);
                found = false;

                foreach (char c in cars)
                {
                    if (c == car)
                    {
                        i++;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    break;
                }
            }

            return i;
        }

        public static string ReadString(string exp, ref int position, char end)
        {
            int i = position;
            int taille = exp.Length;
            char car;
            StringBuilder builder = new StringBuilder();
            int nb = 0;

            while (i < taille)
            {
                car = exp.ElementAt(i);

                if (car == '\\')
                {
                    nb = 0;
                    i++;
                    while (i < taille)
                    {
                        car = exp.ElementAt(i);
                        if (car == '\\')
                        {
                            if (nb % 2 == 0)
                            {
                                builder.Append(car);
                            }
                        }
                        else if (car == end && nb % 2 == 1)
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            builder.Append(car);
                            break;
                        }

                        nb++;
                        i++;
                    }
                }
                else if (car == end)
                {
                    break;
                }
                else
                {
                    builder.Append(car);
                }

                i++;
            }

            position = i;
            return builder.ToString();
        }

        public static string ReadWord(string exp, int position)
        {
            int i = position;
            int taille = exp.Length;
            char car;
            StringBuilder builder = new StringBuilder();

            while (i < taille)
            {
                car = exp.ElementAt(i);

                if (car == ' ' || car == '\n' || car == '\t' || car == ')'
                        || car == '(' || car == ',' || car == '\r')
                {
                    break;
                }

                builder.Append(car);

                i++;
            }
            return builder.ToString();
        }
    }
}

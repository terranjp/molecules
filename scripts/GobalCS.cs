using System;
using Godot;
public class GlobalCS : Node
{

    public Molecule? MainMolecule;

    [Signal]
    public delegate void MainMoleculResized(string w);

}
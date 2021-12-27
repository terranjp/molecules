using Godot;
using System;
using System.Collections.Generic;

public class Molecule : RigidBody2D
{


    [Export] public bool IsMain = false;

    protected float _radius = 10;

    [Export]
    public float Radius
    {
        get => _radius;
        set => SetRadius(value);
    }

    protected float MoleculeMass = 10;
    protected const float MinPropellingMass = (float)0.0008;
    protected const float MaxPropellingMass = (float)0.0016;
    protected const float SmallPropellingForce = 150;
    protected const float LargePropellingForce = 3;

    private readonly Color _colorVectorMin = new Color(1, 0.25F, 0);
    private readonly Color _colorVectorMax = new Color(0, 0.75F, 1);
    private CollisionShape2D _shape;
    private Area2D _areaNode;
    private CollisionShape2D _areaShape;
    private Mesh _mesh;
    protected PackedScene _moleculeScene;
    private GDScript global;


    public override void _Ready()
    {
        _shape = GetNode<CollisionShape2D>("Shape");
        _areaNode = GetNode<Area2D>("Area");
        _areaShape = GetNode<CollisionShape2D>("Area/Shape");
        // _mesh = GetNode<Mesh>("Mesh");
        _moleculeScene = GD.Load<PackedScene>("res://scenes/Molecule.tscn");

        // ShaderMaterial mat = (ShaderMaterial)_mesh.SurfaceGetMaterial(0).Duplicate(false);
        // mat.SetShaderParam("offset", GD.Randf());
        _shape.Shape = new CircleShape2D();
        _areaShape.Shape = new CircleShape2D();

        global = (GDScript)GD.Load("res://scripts/global.gd");


        // var main = global.MainMolecule;

        // _global 


        if (IsMain)
        {
            global.Set("main_molecule", this);
            // Global.MainMolecule = this;
            // global.MainMolecule = this;
            SetRadius(_radius);
        }
        else
        {
            GetNode("/root/Global").Connect("main_molecule_resized", this, nameof(AdjustColor));
            // Connect("main_molecule_resized", this, nameof(AdjustColor));
            SetRadius(_radius);
            AdjustColor();
        }
    }

    public override void _Process(float delta)
    {
        var overlappingMolecules = new List<Molecule>();

        // foreach (Area2D a in _areaNode.GetOverlappingAreas())
        // {
        //     var molecule = (Molecule)a.GetParent();
        //     if (molecule.Radius < Radius && molecule.Radius >= 0)
        //         overlappingMolecules.Add(molecule);
        // }
        //
        // foreach (var small in overlappingMolecules)
        // {
        //     var distance = Position.DistanceTo(small.Position);
        //     var radiusDifference = Radius + small.Radius - distance;
        //     if (radiusDifference < 0)
        //         continue;
        //
        //     var smallRadiusReduced = Math.Max(0, small.Radius - radiusDifference);
        //     var smallMassReduced = RadiusToMass(smallRadiusReduced);
        //     var massDelta = small.MoleculeMass - smallMassReduced;
        //     small.Radius = smallMassReduced;
        //     AddMass(massDelta, small.LinearVelocity);
        // }
    }

    private void AddMass(float addedMass, Vector2 massLinearVelocity)
    {
        if (addedMass <= 0)
            return;

        var newMass = MoleculeMass + addedMass;
        var newVelocity = (LinearVelocity * MoleculeMass / newMass + massLinearVelocity * addedMass / newMass);
        var newRadius = MassToRadius(newMass);
        _radius = newRadius;
        LinearVelocity = newVelocity;
    }


  


 
    private void SetRadius(float value)
    {
        _radius = value;
        MoleculeMass = RadiusToMass(value);
        if (IsInsideTree())
        {
            var shape = (CircleShape2D)_shape.Shape;
            var area_shape = (CircleShape2D)_areaShape.Shape;
            float scale = value * 2;
            // Mesh.Scale = Vector2(scale, scale);
            area_shape.Radius = value;
            shape.Radius = value;
            EmitSignal("main_molecule_resized");
        }
    }

    private void AdjustColor()
    {
        float c = 1.0F;


        // if (Global.MainMolecule != null && Global.MainMolecule.Radius > 0)
        // {
        //     c = Radius / Global.MainMolecule.Radius * 2;
        //     Mathf.Clamp(c, 0, 1);
        // }

        // var colorVector = _colorVectorMin * c + _colorVectorMax * (1 - c);


    }

    private float RadiusToMass(float radius)
    {
        return (float)(Math.PI * Math.Pow(radius, 2));
    }


    protected float MassToRadius(float mass)
    {
        return (float)(Math.Sqrt(mass / Math.PI));
    }

}

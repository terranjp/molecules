using Godot;
using System;
using System.Collections.Generic;


public class Molecule : RigidBody2D
{

	[Export] public bool IsMain = false;

	private float _radius = 30;

	[Export]
	public float Radius
	{
		get => _radius;
		set => SetRadius(value);
	}

	[Export] public float MoleculeMass = 10;

	private readonly Color _colorVectorMin = new Color(1, 0.25F, 0);
	private readonly Color _colorVectorMax = new Color(0, 0.75F, 1);
	private CollisionShape2D _shape;
	private Area2D _areaNode;
	private CollisionShape2D _areaShape;
	private MeshInstance2D _mesh;
	protected PackedScene _moleculeScene;
	protected GDScript global;


	public override void _Ready()
	{
		_shape = GetNode<CollisionShape2D>("Shape");
		_areaNode = GetNode<Area2D>("Area");
		_areaShape = GetNode<CollisionShape2D>("Area/Shape");
		_mesh = GetNode<MeshInstance2D>("Mesh");
		var mesh = _mesh.Material.Duplicate() as ShaderMaterial;
		mesh.SetShaderParam("offset", GD.Randf());

		_moleculeScene = GD.Load<PackedScene>("res://scenes/Molecule.tscn");
		_shape.Shape = new CircleShape2D();
		_areaShape.Shape = new CircleShape2D();

		global = (GDScript)GD.Load("res://scripts/global.gd");
		
		SetRadius(_radius);
		AdjustColor();


	}

	public override void _Process(float delta)
	{
		var overlappingMolecules = new List<Molecule>();

		foreach (Area2D area in _areaNode.GetOverlappingAreas())
		{
			
			var _molecule = area.GetParent<Molecule>();
			if (_molecule.Radius < Radius && _molecule.Radius >= 0){
				overlappingMolecules.Add(_molecule);
			}
		}

		foreach(Molecule molecule in overlappingMolecules){
			var distance = Position.DistanceTo(molecule.Position);
			var radiusDifference = Radius + molecule.Radius - distance;
			if (radiusDifference < 0)
				continue;
		
			var smallRadiusReduced = Math.Max(0, molecule.Radius - radiusDifference);
		
			var smallMassReduced = RadiusToMass(smallRadiusReduced);
			var massDelta = molecule.MoleculeMass - smallMassReduced;
		
			molecule.Radius = smallRadiusReduced;
			AddMass(massDelta, molecule.LinearVelocity);            
		}
	}

	private void AddMass(float addedMass, Vector2 massLinearVelocity)
	{
		if (addedMass <= 0)
			return;

		var newMass = MoleculeMass + addedMass;
		var newVelocity = (LinearVelocity * MoleculeMass / newMass + massLinearVelocity * addedMass / newMass);
		var newRadius = MassToRadius(newMass);
		Radius = newRadius;
		LinearVelocity = newVelocity;
	}


 
	private void SetRadius(float value)
	{
		_radius = value;
		MoleculeMass = RadiusToMass(value);
		if (IsInsideTree())
		{
			
			CircleShape2D shapeCircle = _shape.Shape as CircleShape2D;
			CircleShape2D areaCircle = _areaShape.Shape as CircleShape2D;

			shapeCircle.Radius = value;
			areaCircle.Radius = value;

			float scale = value * 2;
			_mesh.Scale = new Vector2(scale, scale);

			if (IsMain){
				global.Call("emit_signal", "main_molecule_resized");
				EmitSignal("PlayerResized");
			}

		}

		if (Radius <= 0){
			QueueFree();
			if (IsMain){
				global.Set("main_molecule", null);
			}
		}
	}

	private void AdjustColor()
	{
		float c = 1.0F;


		var molecule = global.Get("main_molecule") as Molecule;

		if (molecule!= null && molecule.Radius > 0)
		{
			c = Radius / molecule.Radius * 2;
			c = Mathf.Clamp(c, 0, 1);

		var colorVector = _colorVectorMin * c + _colorVectorMax * (1-c);
		(_mesh.Material as ShaderMaterial).SetShaderParam("color", colorVector);
		}


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

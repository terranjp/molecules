using Godot;

namespace Molecules.scenes
{
	public class Player : Molecule
	{
		[Signal] public delegate void PlayerResized(string value);
		private const float SmallPropellingForce = 150;
		private const float LargePropellingForce = 3;
		private const float MinPropellingMass = (float)0.008;
		private const float MaxPropellingMass = (float)0.016;

		public override void _PhysicsProcess(float delta)
		{

			if (IsMain && Input.IsActionJustPressed("propel"))
				Propel(GetViewport().GetMousePosition() - Position);
		}


		public override void _Ready()
		{
			base._Ready();
			IsMain = true;
			global.Set("main_molecule", this);
			var c = global.Call("emit_signal", "main_molecule_resized");
			EmitSignal("PlayerResized", "some_data");
			GD.Print();
		}


		public void Propel(Vector2 direction)
		{
			if (Radius <= 0)
			{
				return;
			}

			Vector2 directionNorm = direction.Normalized();

			var propellingMass = (float)(MoleculeMass * GD.RandRange(MinPropellingMass, MaxPropellingMass));

			float newMass = (float)(MoleculeMass - propellingMass);

			Radius = MassToRadius(newMass);

			var propellingMolecule = _moleculeScene.Instance() as Molecule;

			propellingMolecule.Radius = MassToRadius(propellingMass);
			propellingMolecule.Position = Position + directionNorm * (Radius + propellingMolecule.Radius * 5);

			propellingMolecule.ApplyCentralImpulse(directionNorm * SmallPropellingForce);

			ApplyCentralImpulse(-directionNorm * LargePropellingForce);

			GetParent().CallDeferred("add_child_below_node", this, propellingMolecule);
		}
		//  }
	}
}

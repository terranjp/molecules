using Godot;

namespace Molecules.scenes
{
    public class Player : Molecule
    {
       

        public override void _PhysicsProcess(float delta)
        {

            if (IsMain && Input.IsActionJustPressed("propel"))
                Propel(GetViewport().GetMousePosition() - Position);
        }
    
        public void Propel(Vector2 direction)
        {

            Vector2 directionNorm = direction.Normalized();

            var propellingMass = MoleculeMass * GD.RandRange(MinPropellingMass, MaxPropellingMass);
            float newMass = (float)(MoleculeMass - propellingMass);
            _radius = MassToRadius(newMass);

            var propellingMolecule = _moleculeScene.Instance() as Molecule;
            propellingMolecule.Radius = MassToRadius(newMass);
            propellingMolecule.Position = Position + directionNorm * (Radius + propellingMolecule.Radius * 5);

            propellingMolecule.ApplyCentralImpulse(directionNorm * SmallPropellingForce);
            ApplyCentralImpulse(-directionNorm * LargePropellingForce);

            GetParent().CallDeferred("add_child_below_node", this, propellingMolecule);

            GD.Print("Propel");
        }
//  }
    }
}

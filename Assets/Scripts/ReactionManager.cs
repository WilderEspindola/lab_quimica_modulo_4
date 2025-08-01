using System.Collections.Generic;
using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    public static ReactionManager Instance { get; private set; }

    [System.Serializable]
    public class SerializableCompound
    {
        public string formula; // Ej: "H2O" (sin subíndices Unicode)
        public List<AtomCount> composition;
    }

    [System.Serializable]
    public struct AtomCount
    {
        public string atom;  // Ej: "H", "O", "Zn"
        public int count;
    }

    [System.Serializable]
    public class SerializableChemicalEquation
    {
        public List<SerializableCompound> reactants;
        public List<SerializableCompound> products;
        public string reactionType; // "combinacion", "descomposicion", "desplazamiento"
    }

    public List<SerializableChemicalEquation> allReactions = new List<SerializableChemicalEquation>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        InitializeDefaultReactions();
    }

    private void InitializeDefaultReactions()
    {
        // 1. Combinacion: H2 + O2 -> H2O
        allReactions.Add(new SerializableChemicalEquation
        {
            reactants = new List<SerializableCompound>
            {
                new SerializableCompound { formula = "H2", composition = new List<AtomCount> { new AtomCount { atom = "H", count = 2 } } },
                new SerializableCompound { formula = "O2", composition = new List<AtomCount> { new AtomCount { atom = "O", count = 2 } } }
            },
            products = new List<SerializableCompound>
            {
                new SerializableCompound { formula = "H2O", composition = new List<AtomCount> { new AtomCount { atom = "H", count = 2 }, new AtomCount { atom = "O", count = 1 } } }
            },
            reactionType = "combinacion"
        });

        // 2. Descomposicion: H2O -> H2 + O2
        allReactions.Add(new SerializableChemicalEquation
        {
            reactants = new List<SerializableCompound>
            {
                new SerializableCompound { formula = "H2O", composition = new List<AtomCount> { new AtomCount { atom = "H", count = 2 }, new AtomCount { atom = "O", count = 1 } } }
            },
            products = new List<SerializableCompound>
            {
                new SerializableCompound { formula = "H2", composition = new List<AtomCount> { new AtomCount { atom = "H", count = 2 } } },
                new SerializableCompound { formula = "O2", composition = new List<AtomCount> { new AtomCount { atom = "O", count = 2 } } }
            },
            reactionType = "descomposicion"
        });

        // 3. Desplazamiento: Zn + HCl -> ZnCl2 + H2
        allReactions.Add(new SerializableChemicalEquation
        {
            reactants = new List<SerializableCompound>
            {
                new SerializableCompound { formula = "Zn", composition = new List<AtomCount> { new AtomCount { atom = "Zn", count = 1 } } },
                new SerializableCompound { formula = "HCl", composition = new List<AtomCount> { new AtomCount { atom = "H", count = 1 }, new AtomCount { atom = "Cl", count = 1 } } }
            },
            products = new List<SerializableCompound>
            {
                new SerializableCompound { formula = "ZnCl2", composition = new List<AtomCount> { new AtomCount { atom = "Zn", count = 1 }, new AtomCount { atom = "Cl", count = 2 } } },
                new SerializableCompound { formula = "H2", composition = new List<AtomCount> { new AtomCount { atom = "H", count = 2 } } }
            },
            reactionType = "desplazamiento"
        });
    }

    public SerializableChemicalEquation GetRandomEquation()
    {
        if (allReactions.Count == 0) InitializeDefaultReactions();
        return allReactions[Random.Range(0, allReactions.Count)];
    }
}
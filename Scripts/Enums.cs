public enum WaveEnemyType { 
    Normal, 
    Fast, 
    Slow 
}

public enum EnemyAnimation { 
    Idle, 
    Die 
}

public enum TurretType { 
    normal, 
    bomb, 
    sniper, 
    support 
}

public enum TurretParticle
{
    levelUp,
    bigLevelUp
}

public enum turretSounds {
    spawn, 
    upgrade, 
    bigUpgrade
}

public enum detectionMode { 
    first, 
    last, 
    closest, 
    healthiest 
}

public enum RotationMode { 
    Self,
    Weapon, 
    Guns 
}

public enum DragPhase { 
    start, 
    dragging, 
    end 
}

public enum BuildMode {
    Walkable, 
    Buildable 
}

using DCEP;


public class DistanceConstraint
{
    public Vertex Start { get; set; }
    public Vertex End { get; set; }
    public double MinimumDistance { get; set; }
    public double MaximumDistance { get; set; }

    public DistanceConstraint(Vertex start, Vertex end, double minDistance, double maxDistance)
    {
        Start = start;
        End = end;
        MinimumDistance = minDistance;
        MaximumDistance = maxDistance;
    }
}

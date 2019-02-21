﻿
namespace TestTransform
{
    
    
    public class TestNetTopology
    {
        
        
        // https://stackoverflow.com/questions/46159499/calculate-area-of-polygon-having-wgs-coordinates-using-dotspatial
        // pfff wrong... 
        public static void TestComputeArea()
        {
            // this feature can be see visually here http://www.allhx.ca/on/toronto/westmount-park-road/25/
            string feature = "-79.525542519049552,43.691278124243432 -79.525382520578987,43.691281097414787 -79.525228855617627,43.69124858593392 -79.525096151437353,43.691183664769774 -79.52472799258571,43.690927163079735 -79.525379447437814,43.690771996666641 -79.525602330675355,43.691267524226838 -79.525542519049552,43.691278124243432";
            feature = "47.3612503,8.5351944 47.3612252,8.5342631 47.3610145,8.5342755 47.3610212,8.5345227 47.3606405,8.5345451 47.3606350,8.5343411 47.3604067,8.5343545 47.3604120,8.5345623 47.3604308,8.5352457 47.3606508,8.5352328 47.3606413,8.5348784 47.3610383,8.5348551 47.3610477,8.5352063 47.3612503,8.5351944";

            string[] coordinates = feature.Split(' ');
            // System.Array.Reverse(coordinates);
            
            Wgs84Coordinates[] points = new Wgs84Coordinates[coordinates.Length];
            
            for (int i = 0; i < coordinates.Length; i++)
            {
                double lon = double.Parse(coordinates[i].Split(',')[0]);
                double lat = double.Parse(coordinates[i].Split(',')[1]);
                
                points[i] = new Wgs84Coordinates((decimal)lat, (decimal)lon);
            } // Next i 
            
            double area = CalculateArea(points);
            System.Console.WriteLine(area);
        } // End Sub TestComputeArea 
        
        
        public static double ToRadians(double degrees)
        {
            return degrees / 180.0 * System.Math.PI;
        } // End Function ToRadian 
        
        
        public static void PolygonArea()
        {
            double[][] poly = new double[][]
            {
                new double[] {47.3612503, 8.5351944},
                new double[] {47.3612252, 8.5342631},
                new double[] {47.3610145, 8.5342755},
                new double[] {47.3610212, 8.5345227},
                new double[] {47.3606405, 8.5345451},
                new double[] {47.3606350, 8.5343411},
                new double[] {47.3604067, 8.5343545},
                new double[] {47.3604120, 8.5345623},
                new double[] {47.3604308, 8.5352457},
                new double[] {47.3606508, 8.5352328},
                new double[] {47.3606413, 8.5348784},
                new double[] {47.3610383, 8.5348551},
                new double[] {47.3610477, 8.5352063},
                new double[] {47.3612503, 8.5351944}
            };

            double area = PolygonArea(poly);
            System.Console.WriteLine(area);
        } // End Sub PolygonArea 
        
        
        // https://gis.stackexchange.com/a/816/3997
        public static double PolygonArea(double[][] poly)
        {
            double area = 0.0;
            int len = poly.Length;
            
            if (len > 2)
            {
                double[] p1, p2;
                
                for (int i = 0; i < len - 1; i++)
                {
                    p1 = poly[i];
                    p2 = poly[i + 1];
                    
                    area += ToRadians(p2[0] - p1[0]) *
                        (
                            2
                            + System.Math.Sin(ToRadians(p1[1]))
                            + System.Math.Sin(ToRadians(p2[1]))
                        );
                } // Next i 
                
                // https://en.wikipedia.org/wiki/Earth_radius#Equatorial_radius
                // https://en.wikipedia.org/wiki/Earth_ellipsoid
                // The radius you are using, 6378137.0 m corresponds to the equatorial radius of the Earth.
                double equatorial_radius = 6378137; // m
                double polar_radius = 6356752.3142; // m
                double mean_radius = 6371008.8; // m
                double authalic_radius = 6371007.2; // m (radius of perfect sphere with same surface as reference ellipsoid)
                double volumetric_radius = 6371000.8; // m (radius of a sphere of volume equal to the ellipsoid)
                // geodetic latitude φ
                double siteLatitude = ToRadians(poly[0][0]);
                
                // https://en.wikipedia.org/wiki/Semi-major_and_semi-minor_axes
                // https://en.wikipedia.org/wiki/World_Geodetic_System
                double a = 6378137; // m
                double b = 6356752.3142; // m
                // where a and b are, respectively, the equatorial radius and the polar radius.
                double R1 = System.Math.Pow(a * a * System.Math.Cos(siteLatitude), 2) +
                         System.Math.Pow(b * b * System.Math.Sin(siteLatitude), 2);
                double R2 = System.Math.Pow(a * System.Math.Cos(siteLatitude), 2) +
                         System.Math.Pow(b * System.Math.Sin(siteLatitude), 2);
                
                // https://en.wikipedia.org/wiki/Earth_radius#Radius_at_a_given_geodetic_latitude
                // Geocentric radius
                double R = System.Math.Sqrt(R1 / R2);
                // double merid_radius = ((a * a) * (b * b)) / Math.Pow(Math.Pow(a * Math.Cos(siteLatitude), 2) + Math.Pow(b * Math.Sin(siteLatitude), 2), 3.0 / 2.0);
                
                
                // System.Console.WriteLine(R);
                // double hrad = polar_radius + (90 - Math.Abs(siteLatitude)) / 90 * (equatorial_radius - polar_radius);
                double radius = mean_radius;
                
                area = area * radius * radius / 2.0;
            } // End if len > 0 
            
            // equatorial_radius: 6391.565558418869 m2
            // mean_radius:       6377.287126172337m2
            // authalic_radius:   6377.283923019292 m2
            // volumetric_radius: 6377.271110415153 m2
            // merid_radius:      6375.314923754325 m2
            // polar_radius:      6348.777989748668 m2
            // R:                 6368.48180842528 m2
            // hrad:              6391.171919886588 m2
            
            // http://postgis.net/docs/doxygen/2.2/dc/d52/geography__measurement_8c_a1a7c48d59bcf4ed56522ab26c142f61d.html
            // ST_Area(false)     6379.25032051953
            // ST_Area(true)      6350.65051177517
            
            // return area;
            return System.Math.Round(area, 2);
        } // End Function PolygonArea 
        
        
        public static double CalculateArea(Wgs84Coordinates[] mycoordinates)
        {
            double[] latLonPoints = new double[mycoordinates.Length * 2];
            double[] z = new double[mycoordinates.Length];
            
            // dotspatial takes the x,y in a single array, and z in a separate array.  I'm sure there's a 
            // reason for this, but I don't know what it is.
            for (int i = 0; i < mycoordinates.Length; i++)
            {
                latLonPoints[i * 2] = (double)mycoordinates[i].Longitude;
                latLonPoints[i * 2 + 1] = (double)mycoordinates[i].Latitude;
                z[i] = 0;
            } // Next i 
            
            
            // source projection is WGS1984
            // https://productforums.google.com/forum/#!msg/gec-data-discussions/FxwUP7bd59g/02tvMDD3vtEJ
            // https://epsg.io/3857
            DotSpatial.Projections.ProjectionInfo projFrom = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;

            // most complicated problem - you have to find most suitable projection
            DotSpatial.Projections.ProjectionInfo projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.UtmWgs1984.WGS1984UTMZone37N;
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.Europe.EuropeAlbersEqualAreaConic; // 6350.9772005155683
            // projTo= DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984; // 5.215560750019806E-07
            // projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.WorldSpheroid.EckertIVsphere; // 6377.26664171461
            // projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.World.EckertIVworld; // 6391.5626849671826
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.World.CylindricalEqualAreaworld; // 6350.6506013739854
            /*
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.WorldSpheroid.CylindricalEqualAreasphere; // 6377.2695087222382
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.WorldSpheroid.EquidistantCylindricalsphere; // 6448.6818862780929
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.World.Polyconicworld; // 8483.7701716953889
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.World.EquidistantCylindricalworld; // 6463.1380225215107
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.World.EquidistantConicworld; // 8197.4427198320627
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.World.VanderGrintenIworld; // 6537.3942984174937
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.World.WebMercator; // 6535.5119516421109
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.World.Mercatorworld; // 6492.7180733950809
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.SpheroidBased.Lambert2; // 9422.0631835013628
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.SpheroidBased.Lambert2Wide; // 9422.0614012926817
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.TransverseMercator.WGS1984lo33; // 6760.01638841012
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.Europe.EuropeAlbersEqualAreaConic; // 6350.9772005155683
            projTo = DotSpatial.Projections.KnownCoordinateSystems.Projected.UtmOther.EuropeanDatum1950UTMZone37N; // 6480.7883094931021
            */
            
            
            // ST_Area(g, false)     6379.25032051953
            // ST_Area(g, true)      6350.65051177517
            // ST_Area(g)            5.21556075001092E-07
            
            
            // prepare for ReprojectPoints (it's mutate array)
            DotSpatial.Projections.Reproject.ReprojectPoints(
                latLonPoints, z, projFrom, projTo
                , 0, latLonPoints.Length / 2
            );
            
            // assemblying new points array to create polygon
            GeoAPI.Geometries.Coordinate[] polyPoints = new GeoAPI.Geometries.Coordinate[latLonPoints.Length/2];
            
            for (int i = 0; i < latLonPoints.Length/2; ++i)
            {
                polyPoints[i] = new GeoAPI.Geometries.Coordinate(latLonPoints[i * 2], latLonPoints[i * 2 + 1]);
            } // Next i 
            
            // Assembling linear ring to create polygon 
            NetTopologySuite.Geometries.LinearRing lr = 
                new NetTopologySuite.Geometries.LinearRing(polyPoints);
            
            if(!lr.IsValid)
                throw new System.IO.InvalidDataException("Coordinates are invalid.");
            
            GeoAPI.Geometries.IPolygon poly = new NetTopologySuite.Geometries.Polygon(lr);
            if(!poly.IsValid)
                throw new System.IO.InvalidDataException("Polygon is invalid.");
            
            return poly.Area;
        } // End Function CalculateArea 
        
        
        public static void Test()
        {
            string s1 = "POLYGON((7.5999034 47.5506347,7.5997595 47.5507183,7.5998959 47.5508256,7.5999759 47.5508885,7.6001195 47.550805,7.5999034 47.5506347))";
            string s2 = "POLYGON((7.6003356 47.5509754,7.6001195 47.550805,7.5999759 47.5508885,7.6000322 47.5509328,7.6001926 47.551059,7.6003356 47.5509754))";
            
            s1 = "POLYGON((7.5999034 47.5506347,7.6001195 47.550805,7.5999759 47.5508885,7.5998959 47.5508256,7.5997595 47.5507183,7.5999034 47.5506347))";
            s2 = "POLYGON((7.6003356 47.5509754,7.6001926 47.551059,7.6000322 47.5509328,7.5999759 47.5508885,7.6001195 47.550805,7.6003356 47.5509754))";
            
            
            // NetTopologySuite.Geometries.Implementation.CoordinateArraySequenceFactory
            // GeoAPI.Geometries.IGeometryFactory geoFactory = new NetTopologySuite.Geometries.GeometryFactory();
            
            
            NetTopologySuite.IO.WKTReader wr = new NetTopologySuite.IO.WKTReader();
            
            Wgs84Coordinates[] coords1 = PolygonParsingExtensions.PolygonStringToCoordinates(s1);
            Wgs84Coordinates[] coords2 = PolygonParsingExtensions.PolygonStringToCoordinates(s2);
            
            var lr = new NetTopologySuite.Geometries.LinearRing(coords1.ToNetTopologyCoordinates());
            System.Console.WriteLine(lr.IsValid);
            
            var x = new NetTopologySuite.Geometries.Polygon(new NetTopologySuite.Geometries.LinearRing(coords1.ToNetTopologyCoordinates()));
            System.Console.WriteLine(x.IsValid);
            
            NetTopologySuite.Geometries.GeometryFactory geomFactory = new NetTopologySuite.Geometries.GeometryFactory();
            
            GeoAPI.Geometries.IPolygon poly1 = geomFactory.CreatePolygon(coords1.ToNetTopologyCoordinates());
            GeoAPI.Geometries.IPolygon poly2 = geomFactory.CreatePolygon(coords2.ToNetTopologyCoordinates());
            
            
            /*
            GeoAPI.Geometries.IPolygon poly1 = (GeoAPI.Geometries.IPolygon)wr.Read(s1);
            GeoAPI.Geometries.IPolygon poly2 = (GeoAPI.Geometries.IPolygon)wr.Read(s2);
            */
            
            poly1.SRID = 4326;
            poly2.SRID = 4326;
            
            
            GeoAPI.Geometries.IPolygon poly3quick = (GeoAPI.Geometries.IPolygon)poly1.Union(poly2);
            System.Console.WriteLine(poly3quick.IsValid);
            
            // https://gis.stackexchange.com/questions/209797/how-to-get-geometry-points-using-geo-api
            System.Console.Write(poly1.IsValid);
            System.Console.Write(poly2.IsValid);
            
            
            System.Collections.Generic.List<GeoAPI.Geometries.IGeometry> lsPolygons =
                new System.Collections.Generic.List<GeoAPI.Geometries.IGeometry>();
            
            lsPolygons.Add(poly1);
            lsPolygons.Add(poly2);
            
            
            GeoAPI.Geometries.IGeometry ig = NetTopologySuite.Operation.Union.CascadedPolygonUnion.Union(lsPolygons);
            System.Console.WriteLine(ig.GetType().FullName);
            
            GeoAPI.Geometries.IPolygon poly3 = (GeoAPI.Geometries.IPolygon)ig;
            System.Console.WriteLine(poly3);
            
            // POLYGON ((7.5997595 47.5507183, 7.5999034 47.5506347, 7.6001195 47.550805, 7.6003356 47.5509754
            // , 7.6001926 47.551059, 7.6000322 47.5509328, 7.5999759 47.5508885
            // , 7.5998959 47.5508256, 7.5997595 47.5507183))
            
            System.Console.WriteLine(poly3.Shell.Coordinates);
            
            
            /*
            // GeoAPI.Geometries.IPolygon poly3 = (GeoAPI.Geometries.IPolygon)ig;
            NetTopologySuite.Geometries.MultiPolygon poly3a = (NetTopologySuite.Geometries.MultiPolygon)ig;
            GeoAPI.Geometries.IGeometry ig2 = poly3a.ConvexHull();
            System.Console.WriteLine(ig2.GetType().FullName);
            */
            
            // GeoAPI.Geometries.IPolygon poly4 = (GeoAPI.Geometries.IPolygon)ig2;
            // System.Console.WriteLine(poly4);
            
            
            System.Console.WriteLine("--- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Test 
        
        
    } // End Class Program 
    
    
} // End Namespace TestTransform 
using SqlReflect.Attributes;

namespace SqlReflectTest.Model {
    [Table("Region")]
    public class Region {
        [PK]
        public int RegionID { get; set; }
        public string RegionDescription { get; set; }


        public override string ToString() {
            return RegionDescription;
        }

        public override bool Equals(object obj) {
            var region = obj as Region;
            return region != null &&
                   RegionID == region.RegionID &&
                   RegionDescription == region.RegionDescription;
        }

        public override int GetHashCode() {
            var hashCode = -931275965;
            hashCode = hashCode * -1521134295 + RegionID.GetHashCode();
            hashCode = hashCode * -1521134295 + RegionDescription.GetHashCode();

            return hashCode;
        }
    }
}

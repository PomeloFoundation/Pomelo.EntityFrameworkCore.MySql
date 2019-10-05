using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal
{
    public class MySqlLoggingDefinitions : RelationalLoggingDefinitions
    {
        public EventDefinitionBase LogDefaultDecimalTypeColumn;

        public EventDefinitionBase LogByteIdentityColumn;

        public EventDefinitionBase LogFoundDefaultSchema;

        public EventDefinitionBase LogFoundTypeAlias;

        public EventDefinitionBase LogFoundColumn;

        public EventDefinitionBase LogFoundForeignKey;

        public EventDefinitionBase LogPrincipalTableNotInSelectionSet;

        public EventDefinitionBase LogMissingSchema;

        public EventDefinitionBase LogMissingTable;

        public EventDefinitionBase LogFoundSequence;

        public EventDefinitionBase LogFoundTable;

        public EventDefinitionBase LogFoundIndex;

        public EventDefinitionBase LogFoundPrimaryKey;

        public EventDefinitionBase LogFoundUniqueConstraint;

        public EventDefinitionBase LogPrincipalColumnNotFound;

        public EventDefinitionBase LogReflexiveConstraintIgnored;
    }
}

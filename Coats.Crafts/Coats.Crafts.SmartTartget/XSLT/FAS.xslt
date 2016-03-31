<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="msxsl sqlite utils"
    xmlns:sqlite="urn:sqlite-database"
    xmlns:utils="urn:extension-utils"
>
  <!--xmlns:cs="urn:schemas-connect-digital"-->
  <!-- exclude-result-prefixes="msxsl cs sqlite" -->
  
  <!--<?xml-stylesheet type="text/xsl" href="cdcatalog.xsl"?>-->

  <!-- + ======================================================================== + -->
  <!-- + Convert XML to XML                                                       + -->
  <!-- + ======================================================================== + -->
  <!-- + Author:  Paul Ashley                                                     + -->
  <!-- + Date:    19/03/2013                                                      + -->
  <!-- + ======================================================================== + -->
  <!-- + Description: Intended to extend the automated FASXML output by           + -->
  <!-- +              SmartTarget by converting the categories into flat sets     + -->
  <!-- + ======================================================================== + -->

  <xsl:output method="xml" indent="yes" encoding="utf-8"/>

  <!-- Key used when finding each UNIQUE category child attribute of 'Facet List' -->
  <!--<xsl:key name="key" match="//attribute[value='Facet List']/attribute/value/text()" use="."/>-->

  <!-- Key used when finding each UNIQUE category child attribute of 'Facet List' -->
  <xsl:key name="hookSize" match="string" use="."/>

  <!-- + ======================================================================== + -->

  <!-- Kick off the matching process -->
  <xsl:template match="/">
    <!-- Recreate the "items" root node -->
    <xsl:element name="items">
      <!-- Apply templates to all "item" child nodes -->
      <xsl:apply-templates select="/items/item"/>
    </xsl:element>
  </xsl:template>

  <!-- Template for each "item" node -->
  <xsl:template match="item">
    <!-- Recreate the "item" node -->
    <xsl:element name="item">

      <!-- Check if the tcmid is present in the SQLite database or not. This will determin an "add" or "update" operation. -->
      <!-- "sqlite" is injected using the c# method AddExtensionObject() -->
      <xsl:variable name="itemOperation" select="sqlite:GetOperation(string(normalize-space(@identifier)), string(normalize-space(@operation)))" />
      <!--<xsl:variable name="itemOperation">add</xsl:variable>-->

      <!-- Copy the existing node attributes -->
      <xsl:attribute name="identifier">
        <xsl:value-of select="@identifier"/>
      </xsl:attribute>
      <xsl:attribute name="operation">
        <xsl:value-of select="$itemOperation"/>
      </xsl:attribute>

      <!-- Get publication Id number ("tcm_0_70_1" => "70") -->
      <xsl:variable name="pubId" select="substring(./attribute[@identifier='publicationid']/value[1], 7, 2)" />

      <!-- Loop through "attribute" child node list -->
      <xsl:for-each select="attribute">

        <!-- Identify the "schematitle" attribute -->
        <xsl:choose>
          <xsl:when test="@identifier='schematitle'">
            <!-- We need to change the schematitle from basetype "text" to "list" -->
            <xsl:call-template name="handleSchemaTitle" />
          </xsl:when>
          <xsl:otherwise>
            <!-- Identify the "componentpresentation" attribute -->
            <xsl:choose>
              <xsl:when test="@identifier='componentpresentation'">
                <!-- This is the bit where we need to do some magic with the componentpresentation (encoded / embedded XML)  -->
                <xsl:call-template name="handleComponentPresentation">
                  <xsl:with-param name="pubId" select="$pubId" />
                </xsl:call-template>
              </xsl:when>
              <xsl:otherwise>
                <!-- Standard "attribute" node, so just output a copy of it -->
                <xsl:copy-of select="." />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>

      </xsl:for-each>

      <!-- Extend the FAS XML by adding new child "attribute" nodes ONLY if the operation is "add"  -->
      <!-- operation can be add/update/delete                                                       -->
      <xsl:choose>
        <xsl:when test="$itemOperation='add'">

          <!-- Add Rating attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">rating</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value">0</xsl:with-param>
            <!-- Needs to handle decimal values (e.g. "2.25") so just go with "text" for now -->
            <!--<xsl:with-param name="type">int</xsl:with-param>-->
            <xsl:with-param name="type">text</xsl:with-param>
          </xsl:call-template>

          <!-- Add Comment Url attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">commenturl</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value">#</xsl:with-param>
          </xsl:call-template>

          <!-- Add Comment Count attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">commentcount</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value">0</xsl:with-param>
            <xsl:with-param name="type">int</xsl:with-param>
          </xsl:call-template>

          <!-- Add Last Comment Date attribute -->
          <!-- This has to be an int to aid FredHopper queries. -->
          <!-- dateTimeMin() should return a string in the format "yyyyMMddHHmm" -->
          <!-- to give an int of, for example, 201304191030-->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">lastcommentdate</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value" select="utils:DateTimeMinInt()" />
            <xsl:with-param name="type">int</xsl:with-param>
          </xsl:call-template>

          <!-- Add Comment 1 attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">comment1</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value">#</xsl:with-param>
          </xsl:call-template>

          <!-- Add Comment By 1 attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">commentby1</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value">#</xsl:with-param>
          </xsl:call-template>

          <!-- Add Comment Date 1 attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">commentdate1</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value" select="utils:DateTimeMin()" />
          </xsl:call-template>

          <!-- Add Comment 2 attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">comment2</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value">#</xsl:with-param>
          </xsl:call-template>

          <!-- Add Comment By 2 attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">commentby2</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value">#</xsl:with-param>
          </xsl:call-template>

          <!-- Add Comment Date 2 attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">commentdate2</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value" select="utils:DateTimeMin()" />
          </xsl:call-template>

          <!-- Add Comment 3 attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">comment3</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value">#</xsl:with-param>
          </xsl:call-template>

          <!-- Add Comment By 3 attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">commentby3</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value">#</xsl:with-param>
          </xsl:call-template>

          <!-- Add Comment Date 3 attribute -->
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">commentdate3</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value" select="utils:DateTimeMin()" />
          </xsl:call-template>

        </xsl:when>
      </xsl:choose>

      <!-- Add these ONLY IF the operation is NOT delete -->
      <xsl:choose>
        <xsl:when test="not($itemOperation='delete')">
          <xsl:call-template name="createFASXMLAttribute">
            <xsl:with-param name="id">publishdate</xsl:with-param>
            <xsl:with-param name="locale" select=".//@locale[1]" />
            <xsl:with-param name="value" select="utils:DateTimeNow()" />
          </xsl:call-template>
        </xsl:when>
      </xsl:choose>

    </xsl:element>
  </xsl:template>

  <!-- Outputs the hierachical categories "attribute" node as is, -->
  <!-- then creates flattened "set" lists accordingly -->
  <xsl:template name="handleCategories">

    <!-- First off, just output a copy of the "attribute" node -->
    <xsl:copy-of select="." />

    <!-- ########################################################################## -->
    <!-- No longer required: facet sets are created from the component presentation -->
    <!-- ########################################################################## -->
    <!--
    - Store the current node position -
    <xsl:variable name="currentNode" select="." />
    
    - Get each UNIQUE category child attribute of 'Facet List' -
    <xsl:variable name="uniqueCategories"
        select="//attribute[value='Facet List']/attribute/value/text()[generate-id()=generate-id(key('key',.)[1])]">
    </xsl:variable>

    - Loop through each UNIQUE category child attributes of 'Facet List' -
    <xsl:for-each select="$uniqueCategories">

      <xsl:variable name="uniqueName" select="." />
      
      - Call the template to actually create the facet set list -
      <xsl:call-template name="createIndividualFacetSet">
        <xsl:with-param name="nodeList" select="$currentNode//attribute[value=$uniqueName[1]]"/>
        <xsl:with-param name="setId" select="$uniqueName"/>
      </xsl:call-template>

    </xsl:for-each>
    -->
    <!-- ########################################################################## -->

  </xsl:template>

  <!-- Outputs the componentpresentation "attribute" node as is, -->
  <!-- then creates a few extra "attribute" nodes by breaking    -->
  <!-- out the embedded schema fields. This is to aid FredHopper -->
  <!-- when creating search passes and search profiles.          -->
  <xsl:template name="handleComponentPresentation">
    <xsl:param name="pubId"></xsl:param>

    <!--<xsl:variable name="safeComPres" sselect="translate(., '', ''')"-->

    <!-- First off, just output a copy of the "attribute" node.    -->
    <!-- What could be simpler, eh???                              -->
    <!-- Well, something is screwing up, so need a different tack. -->
    <xsl:copy-of select="." />

    <!-- Store the locale for future set building -->
    <xsl:variable name="locale" select="./name/@locale"/>
    
    <!-- Parse the componentpresentation xml string into an xml node list -->
    <xsl:variable name="parsedXml" select="utils:Parse(string(normalize-space(./value)))"/>

    <!-- Look for embedded values: -->
    <!-- Generic... -->
    <!-- + title -->
    <xsl:call-template name="createFASXMLAttribute">
      <xsl:with-param name="id">title</xsl:with-param>
      <!--<xsl:with-param name="locale" select="./name/@locale" />-->
      <xsl:with-param name="locale" select="$locale" />
      <xsl:with-param name="value" select="$parsedXml/Component/Fields//Field[Name='title']/Values/string" />
      <xsl:with-param name="pubId" select="$pubId" />
      <!--<xsl:with-param name="noName">true</xsl:with-param>-->
    </xsl:call-template>

    <!-- + sort_by_title -->
    <!-- Special case to enable sort by on title attribute because FH refused to search on an attribute id that begins with a digit -->
    <!-- e.g. "78_title" -->
    <xsl:call-template name="createFASXMLAttribute">
      <xsl:with-param name="id">sort_by_title</xsl:with-param>
      <xsl:with-param name="locale" select="$locale" />
      <xsl:with-param name="value" select="$parsedXml/Component/Fields//Field[Name='title']/Values/string" />
    </xsl:call-template>

    <!-- + alt_title -->
    <xsl:call-template name="createFASXMLAttribute">
      <xsl:with-param name="id">alt_title</xsl:with-param>
      <!--<xsl:with-param name="locale" select="./name/@locale" />-->
      <xsl:with-param name="locale" select="$locale" />
      <xsl:with-param name="value" select="$parsedXml/Component/Fields//Field[Name='alt_title']/Values/string" />
      <xsl:with-param name="pubId" select="$pubId" />
    </xsl:call-template>

    <!-- + summary -->
    <xsl:call-template name="createFASXMLAttribute">
      <xsl:with-param name="id">summary</xsl:with-param>
      <!--<xsl:with-param name="locale" select="./name/@locale" />-->
      <xsl:with-param name="locale" select="$locale" />
      <xsl:with-param name="value" select="$parsedXml/Component/Fields//Field[Name='summary']/Values/string" />
      <xsl:with-param name="pubId" select="$pubId" />
    </xsl:call-template>

    <!-- + featured content... -->
    <xsl:variable name="featuredContent" select="$parsedXml/Component/MetadataFields/item/value/Field/LinkedComponentValues/Component/Fields/item[key/string='featuredcontent']" />
    <xsl:variable name="featuredContentVal">
      <xsl:choose>
        <xsl:when test="$featuredContent/value/Field/Values[1]/string='Yes'">1</xsl:when>
        <xsl:otherwise>0</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:call-template name="createFASXMLAttribute">
      <xsl:with-param name="id">featuredcontent</xsl:with-param>
      <!--<xsl:with-param name="locale" select="./name/@locale" />-->
      <xsl:with-param name="locale" select="$locale" />
      <xsl:with-param name="value" select="$featuredContentVal" />
      <xsl:with-param name="type">int</xsl:with-param>
    </xsl:call-template>
    <!-- ...Generic -->

    <!-- Article specific... -->
    <!-- + writtenBy (author) -->
    <xsl:call-template name="createFASXMLAttribute">
      <xsl:with-param name="id">author</xsl:with-param>
      <!--<xsl:with-param name="locale" select="./name/@locale" />-->
      <xsl:with-param name="locale" select="$locale" />
      <xsl:with-param name="value" select="$parsedXml/Component/Fields//Field[Name='by']/EmbeddedValues//Field[Name='author']/Values/string" />
      <xsl:with-param name="pubId" select="$pubId" />
    </xsl:call-template>
    <!-- ...Article specific -->

    <!-- Designer specific... -->
    <!-- + designer -->
    <xsl:call-template name="createFASXMLAttribute">
      <xsl:with-param name="id">designer</xsl:with-param>
      <!--<xsl:with-param name="locale" select="./name/@locale" />-->
      <xsl:with-param name="locale" select="$locale" />
      <xsl:with-param name="value" select="$parsedXml/Component/Fields//Field[Name='designer']/Values/string" />
      <xsl:with-param name="pubId" select="$pubId" />
    </xsl:call-template>
    <!-- ...Designer specific -->

    <!-- Facet sets -->
    <!-- Create the outer "attribute" set container -->
    <xsl:variable name="categoryAncestor">
      <xsl:choose>
        <xsl:when test="count($parsedXml/Component/MetadataFields//Category) > 0">
          <xsl:copy-of select="$parsedXml/Component/MetadataFields"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:copy-of select="$parsedXml/Component/Categories"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <!-- Special case for "Ticket" and "Crochet Hook Size"... -->
    <!-- THIS COULD REALLY DO WITH BEING GENERICISED, BUT IT'S THE LAST DAY OF THE BUILD!!! -->

    <xsl:variable name="tickets" select="$parsedXml/Component/Fields/item/value/Field/LinkedComponentValues/Component[Schema/Title='Crafts.Product.NeedleCraft.ArticleTicket']"/>

    <!-- + "tickets" set... -->
    <xsl:choose>
      <xsl:when test="count($tickets/Fields/item[key/string='ticket']/value/Field/Values/string) > 0">

        <xsl:element name="attribute">

          <xsl:attribute name="identifier">
            <xsl:value-of select="concat($pubId, '_tickets')" />
          </xsl:attribute>
          <xsl:attribute name="type">set</xsl:attribute>

          <!-- Create the inner "value" nodes -->
          <xsl:for-each select="$tickets/Fields/item[key/string='ticket']">
            <xsl:element name="value">
              <xsl:attribute name="identifier">
                <!-- Convert to lower case and replace spaces with underscore -->
                <xsl:call-template name="toLower">
                  <xsl:with-param name="src" select="concat($pubId, concat('_tickets__dir__', ./value/Field/Values/string))" />
                </xsl:call-template>
              </xsl:attribute>
              <xsl:value-of select="./value/Field/Values/string" />
            </xsl:element>
          </xsl:for-each>

        </xsl:element>

      </xsl:when>
    </xsl:choose>

    <!-- + "crochet_hook_sizes" set -->
    <xsl:choose>
      <xsl:when test="count($tickets/Fields/item[key/string='crochet_hook_sizes']/value/Field/Values/string) > 0">
        <xsl:element name="attribute">

          <xsl:attribute name="identifier">
            <xsl:value-of select="concat($pubId, '_crochet_hook_sizes')" />
          </xsl:attribute>
          <xsl:attribute name="type">set</xsl:attribute>

          <!-- Create the inner "value" nodes (generate-id is used to get unique values) -->
          <xsl:for-each select="$tickets/Fields/item[key/string='crochet_hook_sizes']/value/Field/Values/string[generate-id()=generate-id(key('hookSize',.)[1])]">
            <xsl:element name="value">
              <xsl:attribute name="identifier">
                <!-- Convert to lower case and replace spaces with underscore -->
                <xsl:call-template name="toLower">
                  <xsl:with-param name="src" select="concat($pubId, concat('_crochet_hook_sizes__dir__', .))" />
                </xsl:call-template>
              </xsl:attribute>
              <xsl:value-of select="." />
            </xsl:element>
          </xsl:for-each>

        </xsl:element>
      </xsl:when>
    </xsl:choose>
    <!-- Create the outer "attribute" set container -->


    <!-- ...Special case for "Ticket" and "Crochet Hook Size" -->

    <xsl:for-each select="msxsl:node-set($categoryAncestor)//Category">

      <!--<xsl:call-template name ="createIndividualFacetSet">
        <xsl:with-param name="setId" select="./Title" />
        <xsl:with-param name="nodeList" select="./Keywords/Keyword" />
      </xsl:call-template>-->

      <xsl:call-template name ="nestedFacetSet">
        <xsl:with-param name="nodeList" select="./Keywords/Keyword" />
        <xsl:with-param name="locale" select="$locale" />
      </xsl:call-template>

    </xsl:for-each>

    <!-- Create a "Super Facet" which contains every facet created above.       -->
    <!-- This is because we can then perform "OR" operands on ALL facet items.  -->
    <!-- Example of the facet sets we need to create -->
    <!-- WHAT ABOUT LOCALE??? -->
    <!--
    <attribute identifier="70_super_facet" type="set">
      <value identifier="crochet">Crochet</value>
      <value identifier="knitting">Knitting</value>
    </attribute>
    -->

    <!-- Create the outer "attribute" set container -->
	<xsl:if test="count(msxsl:node-set($categoryAncestor)//Category) > 0">
		<xsl:element name="attribute">
		  <xsl:attribute name="identifier">
			<xsl:value-of select="concat($pubId, '_super_facet')" />
		  </xsl:attribute>
		  <xsl:attribute name="type">set</xsl:attribute>

		  <xsl:element name="name">
			<xsl:attribute name="locale">
			  <xsl:value-of select="$locale"/>
			</xsl:attribute>Super Facet</xsl:element>
		  
		  <xsl:for-each select="msxsl:node-set($categoryAncestor)//Category">

			<xsl:call-template name ="nestedFacetSetAttributes">
			  <xsl:with-param name="nodeList" select="./Keywords/Keyword" />
			  <xsl:with-param name="locale" select="$locale" />
			</xsl:call-template>

		  </xsl:for-each>

		  <!-- + "tickets" set... -->
		  <xsl:choose>
			<xsl:when test="count($tickets/Fields/item[key/string='ticket']/value/Field/Values/string) > 0">

			  <!-- Create the inner "value" nodes -->
			  <xsl:for-each select="$tickets/Fields/item[key/string='ticket']">
				<xsl:element name="value">
				  <xsl:attribute name="identifier">
					<!-- Convert to lower case and replace spaces with underscore -->
					<xsl:call-template name="toLower">
					  <xsl:with-param name="src" select="concat($pubId, concat('_tickets__dir__', ./value/Field/Values/string))" />
					</xsl:call-template>
				  </xsl:attribute>
				  <xsl:value-of select="./value/Field/Values/string" />
				</xsl:element>
			  </xsl:for-each>

			</xsl:when>
		  </xsl:choose>

		  <!-- + "crochet_hook_sizes" set -->
		  <xsl:choose>
			<xsl:when test="count($tickets/Fields/item[key/string='crochet_hook_sizes']/value/Field/Values/string) > 0">

			  <!-- Create the inner "value" nodes (generate-id is used to get unique values) -->
			  <xsl:for-each select="$tickets/Fields/item[key/string='crochet_hook_sizes']/value/Field/Values/string[generate-id()=generate-id(key('hookSize',.)[1])]">
				<xsl:element name="value">
				  <xsl:attribute name="identifier">
					<!-- Convert to lower case and replace spaces with underscore -->
					<xsl:call-template name="toLower">
					  <xsl:with-param name="src" select="concat($pubId, concat('_crochet_hook_sizes__dir__', .))" />
					</xsl:call-template>
				  </xsl:attribute>
				  <xsl:value-of select="." />
				</xsl:element>
			  </xsl:for-each>

			</xsl:when>
		  </xsl:choose>

		</xsl:element>
	</xsl:if>
  </xsl:template>


  <xsl:template name="createIndividualFacetSet">
    <xsl:param name="nodeList"/>
    <xsl:param name="setId"/>

    <!-- Example of the facet sets we need to create -->
    <!-- WHAT ABOUT LOCALE??? -->
    <!--
    <attribute identifier="by_technique" type="set">
      <value identifier="crochet">Crochet</value>
      <value identifier="knitting">Knitting</value>
    </attribute>
    -->

    <xsl:choose>
      <xsl:when test="count($nodeList) > 0">

        <!-- Create the outer "attribute" set container -->
        <xsl:element name="attribute">

          <xsl:attribute name="identifier">
            <!-- Convert to lower case and replace spaces with underscore -->
            <xsl:call-template name="toLower">
              <!--<xsl:with-param name="src" select="./Title" />-->
              <xsl:with-param name="src" select="$setId" />
            </xsl:call-template>
          </xsl:attribute>
          <xsl:attribute name="type">set</xsl:attribute>

          <!-- Create the inner "value" nodes -->
          <!--<xsl:for-each select="./Keywords/Keyword">-->
          <xsl:for-each select="$nodeList">
            <xsl:element name="value">
              <xsl:attribute name="identifier">
                <!-- Convert to lower case and replace spaces with underscore -->
                <xsl:call-template name="toLower">
                  <xsl:with-param name="src" select="./Title" />
                </xsl:call-template>
              </xsl:attribute>
              <xsl:value-of select="./Title" />
            </xsl:element>
          </xsl:for-each>

        </xsl:element>

      </xsl:when>
    </xsl:choose>

  </xsl:template>

  <xsl:template name="toLower">
    <xsl:param name="src" />
    <!-- Convert to lower case and replace spaces with underscore -->
    <xsl:value-of select="translate($src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ', 'abcdefghijklmnopqrstuvwxyz_')"/>
  </xsl:template>

  <xsl:template name="createFASXMLAttribute">
    <xsl:param name="id"/>
    <xsl:param name="type"/>
    <xsl:param name="locale"/>
    <xsl:param name="value"/>
    <xsl:param name="pubId"/>
    <xsl:param name="noName"/>

    <xsl:variable name="attrType">
      <xsl:choose>
        <xsl:when test="string-length($type) > 0">
          <xsl:value-of select="$type"/>
        </xsl:when>
        <xsl:otherwise>text</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:choose>
      <xsl:when test="string-length($value) > 0">
        <xsl:variable name="fullId">
          <xsl:choose>
            <xsl:when test="string-length($pubId) > 0">
              <xsl:value-of select="concat(concat($pubId, '_'), $id)" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="$id"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:element name="attribute">
          <xsl:attribute name="identifier">
            <!-- Convert to lower case and replace spaces with underscore -->
            <xsl:call-template name="toLower">
              <!--<xsl:with-param name="src" select="$id" />-->
              <xsl:with-param name="src" select="$fullId" />
            </xsl:call-template>
          </xsl:attribute>
          <xsl:attribute name="type">
            <xsl:value-of select="$attrType"/>
          </xsl:attribute>
          <xsl:choose>
            <xsl:when test="not($noName = 'true')">
              <xsl:element name="name" xml:space="default">
                <xsl:attribute name="locale">
                  <xsl:value-of select="$locale" />
                </xsl:attribute>
                <xsl:value-of select="$id" />
              </xsl:element>
            </xsl:when>
          </xsl:choose>
          <xsl:element name="value">
            <xsl:value-of select="$value" />
          </xsl:element>
        </xsl:element>
      </xsl:when>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="nestedFacetSet">
    <xsl:param name="nodeList"/>
    <xsl:param name="locale"/>

    <!-- Example of what should be passed in -->
    <!--
    <Keyword Description="Baby" Key="PP002" TaxonomyId="tcm:70-4053-512" Path="\Themes\People\Baby (PP)">
      <Id>tcm:70-19215-1024</Id>
      <Title>Baby (PP)</Title>
      <MetadataFields />
    </Keyword>
    -->

    <xsl:choose>
      <xsl:when test="count($nodeList) > 0">

        <!--<xsl:variable name="allPaths" select="$nodeList//@Path" />
        <xsl:variable name="nestedFacets" select="utils:CreateNested(msxsl:node-set($allPaths), 5)"/>-->

        <xsl:variable name="nestedFacets" select="utils:CreateNested(msxsl:node-set($nodeList), 5, $locale)"/>

        <xsl:for-each select="$nestedFacets/sets//attribute">
          <xsl:copy-of select="." />
        </xsl:for-each>

      </xsl:when>
    </xsl:choose>

  </xsl:template>

  <xsl:template name="nestedFacetSetAttributes">
    <xsl:param name="nodeList"/>
    <xsl:param name="locale"/>
    
    <!-- Example of what should be passed in -->
    <!--
    <Keyword Description="Baby" Key="PP002" TaxonomyId="tcm:70-4053-512" Path="\Themes\People\Baby (PP)">
      <Id>tcm:70-19215-1024</Id>
      <Title>Baby (PP)</Title>
      <MetadataFields />
    </Keyword>
    -->

    <xsl:choose>
      <xsl:when test="count($nodeList) > 0">

        <!--<xsl:variable name="allPaths" select="$nodeList//@Path" />
        <xsl:variable name="nestedFacets" select="utils:CreateNested(msxsl:node-set($allPaths), 5)"/>-->
        <xsl:variable name="nestedFacets" select="utils:CreateNested(msxsl:node-set($nodeList), 5, $locale)"/>

        <xsl:for-each select="$nestedFacets/sets//value">
          <xsl:copy-of select="." />
        </xsl:for-each>

      </xsl:when>
    </xsl:choose>

  </xsl:template>

  <xsl:template name="handleSchemaTitle">
    <!-- We just want to change the basetype from "text" to "set" -->
    <!--
    <attribute identifier="schematitle" type="text">
      <name locale="en_US">schematitle</name>
      <value>Crafts.Article</value>
    </attribute>    
    -->
    <xsl:element name="attribute">
      <xsl:attribute name="identifier">schematitle</xsl:attribute>
      <xsl:attribute name="type">set</xsl:attribute>
      <xsl:copy-of select="./name"/>
      <!-- We need to generalise the products. The schema title will be something like 'Crafts.Product.HandKnitting'. -->
      <!-- So that we don't have to build up an insanely long FredHopper query, we are simply going to group them     -->
      <!-- all under the generic 'Crafts.Product' virtual schema title.                                               -->
      <xsl:choose>
        <xsl:when test="starts-with(./value, 'Crafts.Product.')">
          <xsl:element name="value">Crafts.Product</xsl:element>
        </xsl:when>
        <xsl:otherwise>
          <xsl:copy-of select="./value"/>
        </xsl:otherwise>
      </xsl:choose>

    </xsl:element>
  </xsl:template>

</xsl:stylesheet>

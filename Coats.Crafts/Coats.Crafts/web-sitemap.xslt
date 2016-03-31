<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:map="http://schemas.microsoft.com/AspNet/SiteMap-File-1.0" exclude-result-prefixes="map">
  <xsl:output method="html" encoding="utf-8" indent="yes"/>
  <xsl:variable name="MaxLevel" select="2"></xsl:variable>

  <xsl:template match="map:siteMap">
    <div class="sitemap row-fluid">
      <xsl:apply-templates select="map:siteMapNode/map:siteMapNode[@showinsitemap = 'Yes']"></xsl:apply-templates>
    </div>
  </xsl:template>

  <xsl:template match="map:siteMapNode">
    <div class="span3">
      <div class="header">
        <a href="{@url}" title="{@title}">
          <xsl:value-of select="@navigationtitle"/>
        </a>
      </div>
      <div class="list">
        <xsl:if test="map:siteMapNode[@showinsitemap = 'Yes']">
          <ul>
            <xsl:for-each select="map:siteMapNode[@showinsitemap = 'Yes']">
              <li>
                <a href="{@url}" title="{@title}">
                  <xsl:value-of select="@navigationtitle"/>
                </a>
              </li>
            </xsl:for-each>
          </ul>
        </xsl:if>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>
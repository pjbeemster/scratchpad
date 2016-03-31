<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
				xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:msxsl="urn:schemas-microsoft-com:xslt"
				xmlns:sm="http://schemas.microsoft.com/AspNet/SiteMap-File-1.0"
        xmlns="http://www.sitemaps.org/schemas/sitemap/0.9"
				exclude-result-prefixes="msxsl sm"
				>
  <xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="no"  />
  <xsl:param name="domain"></xsl:param>


  <xsl:template match="/">
    <urlset>
      <xsl:apply-templates select="//sm:siteMapNode[@showinsitemap='Yes']"></xsl:apply-templates>
    </urlset>
  </xsl:template>

  <xsl:template match="sm:siteMapNode">
    <url>
      <loc>
        <xsl:call-template name="WriteUrl">
          <xsl:with-param name="domain" select="$domain" />
          <xsl:with-param name="url" select="@url" />
        </xsl:call-template>
      </loc>
    </url>
  </xsl:template>

  <xsl:template name="WriteUrl">
    <xsl:param name="domain" />
    <xsl:param name="url" />

    <xsl:variable name="removetilde">
      <xsl:choose>
        <xsl:when test="starts-with($url, '~')">
          <xsl:value-of select="substring($url, 2, string-length($url))"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$url"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name="cleanedurl">
      <xsl:choose>
        <xsl:when test="string-length($removetilde) = 1">
          <xsl:text></xsl:text>
        </xsl:when>
        <xsl:when test="substring($removetilde, string-length($removetilde) - 1, 1) = '/'">
          <xsl:value-of select="substring($removetilde, 1, string-length($removetilde) - 1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$removetilde"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:value-of select="concat($domain,$cleanedurl)"/>
  </xsl:template>
</xsl:stylesheet>